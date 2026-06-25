using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using EventBus;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Catalog.Products.Commands;

public class UpdateProductCommand : IRequest<Result<ProductModel>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = string.Empty;
    public double Price { get; init; }
    public string? ImageUrl { get; init; }
    public string? ImageData { get; init; }
    public Guid CategoryId { get; init; }
    public List<VariantAttribute> Variants { get; init; } = new();
    public bool IsDeleted { get; init; }
}

public class UpdateProductValidator
{
    public static void Validate(UpdateProductCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (Guid.Equals(command.Id, Guid.Empty))
            throw new ProductInvalidDataException("Product Id is required");

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ProductInvalidDataException("Product's name is required");

        if (command.Price < 0)
            throw new ProductInvalidDataException("Price must be positive");

        if (Guid.Equals(command.CategoryId, Guid.Empty))
            throw new ProductInvalidDataException("CategoryId is required");
    }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductModel>>
{
    private readonly IProductRepository<Product> _repository;
    private readonly IRedisService _redisService;
    private readonly ISupabaseService _supabaseService;
    private readonly IEventBus _eventBus;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository<Product> repository,
        IRedisService redisService,
        ISupabaseService supabaseService,
        IEventBus eventBus,
        IProductMapping productMapping,
        ILogger<UpdateProductHandler> logger)
    {
        _repository = repository;
        _redisService = redisService;
        _supabaseService = supabaseService;
        _eventBus = eventBus;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<ProductModel>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        UpdateProductValidator.Validate(request);

        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(Handle), request.Id);
            throw new ProductNotFoundException();
        }

        if (!string.IsNullOrEmpty(request.ImageData))
        {
            var base64Parts = request.ImageData.Split(',');
            if (base64Parts.Length != 2)
                throw new InvalidDataException("Base64 is invalid");

            byte[] imageBytes = Convert.FromBase64String(base64Parts[1]);
            var uploadImageResponse = await _supabaseService.UploadImage(imageBytes);

            if (uploadImageResponse.IsSuccess)
            {
                entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

                await _eventBus.PublishAsync(new DeleteImageEvent
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Url = entity.ImageUrl,
                }, ct);
            }
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.LastUpdatedAt = DateTime.UtcNow;
        entity.Variants = JsonConvert.SerializeObject(request.Variants);
        entity.IsDeleted = request.IsDeleted;

        var result = await _repository.UpdateAsync(entity, ct);
        await _redisService.RemoveByPrefixAsync("product", ct);

        return Result<ProductModel>.Success("Update product success", _productMapping.ToProductModel(result));
    }
}
