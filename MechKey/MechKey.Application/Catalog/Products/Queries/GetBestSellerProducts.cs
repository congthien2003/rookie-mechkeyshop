using Application.Comoon;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace MechKey.Application.Catalog.Products.Queries;

public class GetBestSellerProductsQuery : IRequest<Result<IEnumerable<ProductModel>>>
{
}

public class GetBestSellerProductsHandler : IRequestHandler<GetBestSellerProductsQuery, Result<IEnumerable<ProductModel>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<GetBestSellerProductsHandler> _logger;

    public GetBestSellerProductsHandler(
        IApplicationDbContext dbContext,
        IProductMapping productMapping,
        ILogger<GetBestSellerProductsHandler> logger)
    {
        _dbContext = dbContext;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ProductModel>>> Handle(GetBestSellerProductsQuery request, CancellationToken ct)
    {
        try
        {
            var items = await _dbContext.Products
                .OrderByDescending(p => p.SellCount)
                .Take(4)
                .Include(p => p.Category)
                .ToListAsync(ct);

            return Result<IEnumerable<ProductModel>>.Success("Get Best seller", _productMapping.ToListProductModel(items));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(Handle), ex.Message);
            throw new ProductHandleFailedException();
        }
    }
}
