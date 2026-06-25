using Application.Comoon;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Catalog.Categories.Queries;

public class GetCategoryByIdQuery : IRequest<Result<CategoryModel>>
{
    public Guid Id { get; init; }
}

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryModel>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICategoryMapping _categoryMapping;
    private readonly ILogger<GetCategoryByIdHandler> _logger;

    public GetCategoryByIdHandler(
        IApplicationDbContext dbContext,
        ICategoryMapping categoryMapping,
        ILogger<GetCategoryByIdHandler> logger)
    {
        _dbContext = dbContext;
        _categoryMapping = categoryMapping;
        _logger = logger;
    }

    public async Task<Result<CategoryModel>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(Handle), request.Id);
            throw new CategoryNotFoundException();
        }

        return Result<CategoryModel>.Success("Get category by id success", _categoryMapping.ToCategoryModel(entity));
    }
}
