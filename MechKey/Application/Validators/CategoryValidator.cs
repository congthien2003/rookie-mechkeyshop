using Domain.Exceptions;
using Shared.ViewModels.Category;

namespace Application.Validators
{
    public static class CategoryValidator
    {
        public static void CreateCategoryModelValidate(CreateCategoryModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new CategoryInvalidDataException("Category name is required");
        }

        public static void CategoryModelValidate(CategoryModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (Guid.Equals(model.Id, Guid.Empty))
                throw new CategoryInvalidDataException("Category Id is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new CategoryInvalidDataException("Category name is required");
        }
    }
}
