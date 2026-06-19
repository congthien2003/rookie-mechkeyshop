using Application.Comoon;
using Application.Interfaces;
using Application.Interfaces.IApiClient.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Catalog.Categories.Queries;

public class GetCategoryListQuery : IRequest<Result<PagedResult<CategoryModel>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SearchTerm { get; init; } = "";
}

public class GetCategoryListHandler : IRequestHandler<GetCategoryListQuery, Result<PagedResult<CategoryModel>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly ICategoryMapping _categoryMapping;

    public GetCategoryListHandler(
        IApplicationDbContext dbContext,
        IRedisService redisService,
        ICategoryMapping categoryMapping)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _categoryMapping = categoryMapping;
    }

    public async Task<Result<PagedResult<CategoryModel>>> Handle(GetCategoryListQuery request, CancellationToken ct)
    {
        string key = $"category-{request.Page}-{request.PageSize}";
        var data = _redisService.Get<PagedResult<CategoryModel>>(key);

        if (data is null)
        {
            var query = _dbContext.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(request.SearchTerm));
            }

            int totalCount = await query.CountAsync(ct);

            var items = await query
                .Where(c => c.IsDeleted == false)
                .OrderBy(c => c.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            data = new PagedResult<CategoryModel>
            {
                Items = _categoryMapping.ToListCategoryModel(items),
                TotalItems = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            await _redisService.Set(key, data, 120);
        }

        return Result<PagedResult<CategoryModel>>.Success("Get category list success", data);
    }
}
