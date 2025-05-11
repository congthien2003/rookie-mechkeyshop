using Domain.Entity;
using Shared.ViewModels.Category;

namespace Shared.Mapping
{
    public static class CategoryMapping
    {
        public static CategoryModel ToCategoryModel(Category category)
        {
            return new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                IsDeleted = category.IsDeleted,
                CreatedAt = category.CreatedAt,
                LastUpdatedAt = category.LastUpdatedAt,
                UpdateById = category.UpdateById,
            };
        }

        public static List<CategoryModel> ToListCategoryModel(List<Category> categories)
        {
            var data = new List<CategoryModel>();

            foreach (var item in categories)
            {
                data.Add(ToCategoryModel(item));
            }

            return data;
        }

        public static Category ToCategory(CategoryModel model)
        {
            return new Category
            {
                Id = model.Id,
                Name = model.Name,
                IsDeleted = model.IsDeleted,
                CreatedAt = model.CreatedAt,
                LastUpdatedAt = model.LastUpdatedAt,
                UpdateById = model.UpdateById

            };
        }
    }

}
