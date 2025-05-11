using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Shared.Mapping.Implementations
{
    public class CategoryMapping : ICategoryMapping
    {
        public CategoryModel ToCategoryModel(Category category)
        {
            if (category == null)
                return null;

            return new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                LastUpdatedAt = category.LastUpdatedAt,
                UpdateById = category.UpdateById
            };
        }

        public List<CategoryModel> ToListCategoryModel(List<Category> categories)
        {
            if (categories == null)
                return null;

            return categories.Select(category => ToCategoryModel(category)).ToList();
        }

        public Category ToCategory(CategoryModel model)
        {
            if (model == null)
                return null;

            return new Category
            {
                Id = model.Id,
                Name = model.Name,
                CreatedAt = model.CreatedAt,
                LastUpdatedAt = model.LastUpdatedAt,
                UpdateById = model.UpdateById

            };
        }

        public Category ToCategoryByCreatedCategoryModel(CreateCategoryModel model)
        {
            if (model == null)
                return null;

            return new Category
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = null,
                UpdateById = model.UpdatedById
            };
        }
    }
}