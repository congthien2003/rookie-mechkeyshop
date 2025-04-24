using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Category;

public class CategoryDropdownViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;

    public CategoryDropdownViewComponent(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categoriesResult = await _categoryService.GetAllAsync(new PaginationReqModel { Page = 1, PageSize = 10, SearchTerm = "" });
        if (!categoriesResult.IsSuccess)
        {
            return View(new List<CategoryModel>()); // fallback
        }

        return View(categoriesResult.Data.Items);
    }
}
