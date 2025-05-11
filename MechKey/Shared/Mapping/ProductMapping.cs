using Domain.Entity;
using Newtonsoft.Json;
using Shared.ViewModels.Product;

namespace Shared.Mapping
{
    public static class ProductMapping
    {
        public static ProductModel ToProductModel(Product product)
        {
            return new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product?.Category?.Name,
                Stock = product.Stock,
                SellCount = product.SellCount,
                CreatedAt = product.CreatedAt,
                LastUpdatedAt = product.LastUpdatedAt,
                Variants = !string.IsNullOrEmpty(product.Variants)
                    ? JsonConvert.DeserializeObject<List<VariantAttribute>>(product.Variants)
                    : new List<VariantAttribute>(),
                Rating = ProductRatingMapping.ToListProductRatingModel(product.ProductRatings?.ToList()), // assuming ProductRatingModel == ProductRating
                TotalRating = product.ProductRatings != null && product.ProductRatings.Count > 0
                    ? product.ProductRatings.Average(r => r.Stars)
                    : 0,
                UpdateById = product.UpdateById
            };
        }

        public static List<ProductModel> ToListProductModel(List<Product> products)
        {
            var result = new List<ProductModel>();
            foreach (var product in products)
            {
                result.Add(ToProductModel(product));
            }
            return result;
        }

        public static Product ToProduct(ProductModel model)
        {
            return new Product
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                Stock = model.Stock,
                SellCount = model.SellCount,
                CreatedAt = model.CreatedAt,
                LastUpdatedAt = model.LastUpdatedAt,
                Variants = model.Variants != null
                    ? JsonConvert.SerializeObject(model.Variants)
                    : string.Empty,
                UpdateById = model.UpdateById

            };
        }
    }

}
