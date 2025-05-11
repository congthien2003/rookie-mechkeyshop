using Domain.Entity;
using Shared.ViewModels.Product;

namespace Shared.Mapping
{
    public static class ProductRatingMapping
    {
        public static ProductRatingModel ToProductRatingModel(ProductRating rating)
        {
            return new ProductRatingModel
            {
                Id = rating.Id,
                Stars = rating.Stars,
                Comment = rating.Comment,
                RatedAt = rating.RatedAt,
                ProductId = rating.ProductId,
                UserId = rating.UserId,
                Name = rating.User?.Name ?? string.Empty,
            };
        }

        public static List<ProductRatingModel> ToListProductRatingModel(List<ProductRating> ratings)
        {
            var result = new List<ProductRatingModel>();
            foreach (var rating in ratings)
            {
                result.Add(ToProductRatingModel(rating));
            }
            return result;
        }

        public static ProductRating ToProductRating(ProductRatingModel model)
        {
            return new ProductRating
            {
                Id = model.Id,
                Stars = model.Stars,
                Comment = model.Comment,
                RatedAt = model.RatedAt,
                ProductId = model.ProductId,
                UserId = model.UserId,
            };
        }
    }

}
