using Application.Comoon;
using Application.Interfaces;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace MechKey.Application.Catalog.Products.Queries;

public class GetProductByIdQuery : IRequest<Result<ProductModel>>
{
    public Guid Id { get; init; }
}

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductModel>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<GetProductByIdHandler> _logger;

    public GetProductByIdHandler(
        IApplicationDbContext dbContext,
        IProductMapping productMapping,
        ILogger<GetProductByIdHandler> logger)
    {
        _dbContext = dbContext;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<ProductModel>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var entity = await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (entity == null)
        {
            _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(Handle), request.Id);
            throw new ProductNotFoundException();
        }

        return Result<ProductModel>.Success("Get product by id success", _productMapping.ToProductModel(entity));
    }
}
