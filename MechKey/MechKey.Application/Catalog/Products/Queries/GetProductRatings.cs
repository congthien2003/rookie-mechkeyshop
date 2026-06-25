using Application.Comoon;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace MechKey.Application.Catalog.Products.Queries;

public class GetProductRatingsQuery : IRequest<Result<PagedResult<ProductRatingModel>>>
{
    public Guid ProductId { get; init; }
    public int PageSize { get; init; } = 4;
    public bool AscOrder { get; init; } = false;
}

public class GetProductRatingsHandler : IRequestHandler<GetProductRatingsQuery, Result<PagedResult<ProductRatingModel>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProductRatingMapping _productRatingMapping;

    public GetProductRatingsHandler(
        IApplicationDbContext dbContext,
        IProductRatingMapping productRatingMapping)
    {
        _dbContext = dbContext;
        _productRatingMapping = productRatingMapping;
    }

    public async Task<Result<PagedResult<ProductRatingModel>>> Handle(GetProductRatingsQuery request, CancellationToken ct)
    {
        var query = _dbContext.ProductRatings
            .Where(pr => pr.ProductId == request.ProductId)
            .AsQueryable();

        if (request.AscOrder)
        {
            query = query.OrderBy(pr => pr.RatedAt);
        }
        else
        {
            query = query.OrderByDescending(pr => pr.Id);
        }

        var data = await query
            .Take(request.PageSize)
            .Include(pr => pr.User)
            .Select(pr => _productRatingMapping.ToProductRatingModel(pr))
            .ToListAsync(ct);

        var totalItems = await _dbContext.ProductRatings
            .Where(pr => pr.ProductId == request.ProductId)
            .CountAsync(ct);

        var result = new PagedResult<ProductRatingModel>
        {
            Page = 1,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            Items = data,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };

        return Result<PagedResult<ProductRatingModel>>.Success("Get rating success", result);
    }
}
