using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Shared.Mapping.Interfaces
{
    public interface ICategoryMapping
    {
        CategoryModel ToCategoryModel(Category category);
        List<CategoryModel> ToListCategoryModel(List<Category> categories);
        Category ToCategory(CategoryModel model);
        Category ToCategoryByCreatedCategoryModel(CreateCategoryModel model);

    }
}