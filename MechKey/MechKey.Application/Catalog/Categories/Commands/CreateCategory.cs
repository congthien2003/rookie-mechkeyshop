using Application.Comoon;
using Application.Interfaces;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Catalog.Categories.Commands;

public class CreateCategoryCommand : IRequest<Result<CategoryModel>>
{
    public string Name { get; init; } = default!;
}

public class CreateCategoryValidator
{
    public static void Validate(CreateCategoryCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new CategoryInvalidDataException("Category name is required");
    }
}

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryModel>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly ICategoryMapping _categoryMapping;

    public CreateCategoryHandler(
        IApplicationDbContext dbContext,
        IRedisService redisService,
        ICategoryMapping categoryMapping)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _categoryMapping = categoryMapping;
    }

    public async Task<Result<CategoryModel>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        CreateCategoryValidator.Validate(request);

        var entity = new Category { Id = Guid.NewGuid(), Name = request.Name };
        _dbContext.Categories.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _redisService.RemoveByPrefixAsync("category", ct);

        return Result<CategoryModel>.Success("Add category success", _categoryMapping.ToCategoryModel(entity));
    }
}
