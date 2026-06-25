using Application.Comoon;
using Application.Interfaces;
using Application.Interfaces.IApiClient.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace MechKey.Application.Catalog.Products.Queries;

public class GetProductListQuery : IRequest<Result<PagedResult<ProductModel>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SearchTerm { get; init; } = "";
    public string CategoryId { get; init; } = "";
    public string SortCol { get; init; } = "";
    public bool Asc { get; init; } = true;
}

public class GetProductListHandler : IRequestHandler<GetProductListQuery, Result<PagedResult<ProductModel>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly IProductMapping _productMapping;

    public GetProductListHandler(
        IApplicationDbContext dbContext,
        IRedisService redisService,
        IProductMapping productMapping)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _productMapping = productMapping;
    }

    public async Task<Result<PagedResult<ProductModel>>> Handle(GetProductListQuery request, CancellationToken ct)
    {
        string key = $"product-{request.Page}-{request.PageSize}-{request.CategoryId}-{request.SearchTerm}-{request.SortCol}-{request.Asc}";
        var data = _redisService.Get<PagedResult<ProductModel>>(key);
        if (data is not null)
        {
            return Result<PagedResult<ProductModel>>.Success("Get list product from cache", data);
        }

        var query = _dbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.CategoryId))
        {
            query = query.Where(q => q.Category.Id.ToString() == request.CategoryId);
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u => u.Name.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.SortCol))
        {
            switch (request.SortCol)
            {
                case "price":
                    query = request.Asc ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                    break;
                case "createdAt":
                    query = request.Asc ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(p => p.Id);
                    break;
            }
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        data = new PagedResult<ProductModel>
        {
            Items = _productMapping.ToListProductModel(items),
            TotalItems = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
        };

        await _redisService.Set(key, data, 15);

        return Result<PagedResult<ProductModel>>.Success("Get list product success", data);
    }
}
