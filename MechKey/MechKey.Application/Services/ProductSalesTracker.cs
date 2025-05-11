using Application.Interfaces.IServices;
using Domain.Entity;
using Domain.IRepositories;

namespace Application.Services
{
    public class ProductSalesTracker : IProductSalesTracker
    {
        private readonly IProductRepository<Product> _repository;

        public ProductSalesTracker(IProductRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task ProductIncreaseSellCount(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
        {
            foreach (var item in orderItems)
            {
                var product = await _repository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null) continue;

                product.IncreaseSellCount(item.Quantity);
                await _repository.UpdateAsync(product, cancellationToken);
            }
        }
    }
}
