using Application.Comoon;
using Application.Interfaces;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Catalog.Categories.Commands;

public class UpdateCategoryCommand : IRequest<Result<CategoryModel>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
}

public class UpdateCategoryValidator
{
    public static void Validate(UpdateCategoryCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (Guid.Equals(command.Id, Guid.Empty))
            throw new CategoryInvalidDataException("Category Id is required");

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new CategoryInvalidDataException("Category name is required");
    }
}

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryModel>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly ICategoryMapping _categoryMapping;
    private readonly ILogger<UpdateCategoryHandler> _logger;

    public UpdateCategoryHandler(
        IApplicationDbContext dbContext,
        IRedisService redisService,
        ICategoryMapping categoryMapping,
        ILogger<UpdateCategoryHandler> logger)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _categoryMapping = categoryMapping;
        _logger = logger;
    }

    public async Task<Result<CategoryModel>> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        UpdateCategoryValidator.Validate(request);

        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(Handle), request.Id);
            throw new CategoryNotFoundException();
        }

        entity.Name = request.Name;
        entity.LastUpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
        await _redisService.RemoveByPrefixAsync("category", ct);

        return Result<CategoryModel>.Success("Update category success", _categoryMapping.ToCategoryModel(entity));
    }
}
