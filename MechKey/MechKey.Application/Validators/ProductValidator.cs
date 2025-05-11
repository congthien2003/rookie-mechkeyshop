using Domain.Exceptions;
using Shared.ViewModels.Product;

namespace Application.Validators
{
    public static class ProductValidator
    {
        public static void CreatedProductValidator(CreateProductModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ProductInvalidDataException("Product's name is required");

            if (Guid.Equals(model.CategoryId, Guid.Empty))
                throw new ProductInvalidDataException("Product's categoryId is required");

            // Optional: If ImageData is provided, it must be in valid base64 format
            if (string.IsNullOrEmpty(model.ImageData))
                throw new ProductInvalidDataException("Product's image data is required");

            if (model.Price < 0)
                throw new ProductInvalidDataException("Price must be positive");
        }

        public static void UpdatedProductValidator(UpdateProductModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (Guid.Equals(model.Id, Guid.Empty))
                throw new ProductInvalidDataException("Product Id is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ProductInvalidDataException("Product's name is required");

            if (model.Price < 0)
                throw new ProductInvalidDataException("Price must be positive");

            if (Guid.Equals(model.CategoryId, Guid.Empty))
                throw new ProductInvalidDataException("CategoryId is required");
        }

        public static void ProductRatingModelValidate(ProductRatingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (model.Stars < 1 || model.Stars > 5)
                throw new ProductRatingInvalidDataException("Stars must be between 1 and 5");

            if (string.IsNullOrWhiteSpace(model.Comment))
                throw new ProductRatingInvalidDataException("Comment cannot be empty");

            if (Guid.Equals(model.ProductId, Guid.Empty))
                throw new ProductRatingInvalidDataException("ProductId is required");

            if (Guid.Equals(model.UserId, Guid.Empty))
                throw new ProductRatingInvalidDataException("UserId is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ProductRatingInvalidDataException("User name is required");
        }
    }

}
