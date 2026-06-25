using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IUnitOfWork;
using Domain.Entity;
using Domain.Exceptions;
using EventBus;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Catalog.Products.Commands;

public class CreateProductCommand : IRequest<Result<ProductModel>>
{
    public string Name { get; init; } = default!;
    public string Description { get; init; } = string.Empty;
    public double Price { get; init; }
    public Guid CategoryId { get; init; }
    public List<VariantAttribute> Variants { get; init; } = new();
    public string ImageData { get; init; } = default!;
}

public class CreateProductValidator
{
    public static void Validate(CreateProductCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ProductInvalidDataException("Product's name is required");

        if (Guid.Equals(command.CategoryId, Guid.Empty))
            throw new ProductInvalidDataException("Product's categoryId is required");

        if (string.IsNullOrEmpty(command.ImageData))
            throw new ProductInvalidDataException("Product's image data is required");

        if (command.Price < 0)
            throw new ProductInvalidDataException("Price must be positive");
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductModel>>
{
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly ISupabaseService _supabaseService;
    private readonly IEventBus _eventBus;
    private readonly IRedisService _redisService;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductUnitOfWork unitOfWork,
        ISupabaseService supabaseService,
        IEventBus eventBus,
        IRedisService redisService,
        IProductMapping productMapping,
        ILogger<CreateProductHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _supabaseService = supabaseService;
        _eventBus = eventBus;
        _redisService = redisService;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<ProductModel>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        CreateProductValidator.Validate(request);

        var base64Parts = request.ImageData.Split(',');
        if (base64Parts.Length != 2)
            throw new InvalidDataException("Base64 is invalid");

        byte[] imageBytes = Convert.FromBase64String(base64Parts[1]);
        var uploadImageResponse = await _supabaseService.UploadImage(imageBytes);

        if (!uploadImageResponse.IsSuccess)
            throw new ProductImageHandleFailedException();

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            var entity = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Price = request.Price,
                Variants = JsonConvert.SerializeObject(request.Variants),
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            ProductImage productImage = new ProductImage
            {
                FilePath = uploadImageResponse.Data.FilePath,
                Url = uploadImageResponse.Data.PublicUrl,
                ProductId = entity.Id,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

            var newEntity = await _unitOfWork.ProductRepository.CreateAsync(entity, ct);

            await _unitOfWork.CommitAsync(ct);
            await _redisService.RemoveByPrefixAsync("product", ct);
            return Result<ProductModel>.Success("Add product success", _productMapping.ToProductModel(newEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {Method}. Model: {Model}, Message: {Message}", nameof(Handle), request, ex.Message);

            await _eventBus.PublishAsync(new DeleteImageEvent
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Url = uploadImageResponse.Data.PublicUrl
            }, ct);

            await _unitOfWork.RollbackAsync(ct);

            throw new ProductHandleFailedException();
        }
    }
}
