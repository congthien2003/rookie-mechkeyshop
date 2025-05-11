using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Shared.Mapping.Interfaces
{
    public interface IProductRatingMapping
    {
        ProductRatingModel ToProductRatingModel(ProductRating rating);
        List<ProductRatingModel> ToListProductRatingModel(List<ProductRating> ratings);
        ProductRating ToProductRating(ProductRatingModel model);
    }
}