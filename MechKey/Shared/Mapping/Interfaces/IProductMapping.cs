using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Shared.Mapping.Interfaces
{
    public interface IProductMapping
    {
        ProductModel ToProductModel(Product product);
        List<ProductModel> ToListProductModel(List<Product> products);
        Product ToProduct(ProductModel model);
    }
}