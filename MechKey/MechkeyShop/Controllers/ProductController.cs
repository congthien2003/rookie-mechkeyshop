using Application.Interfaces.IServices;
using MechkeyShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace MechkeyShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> IndexAsync(ProductPageViewModel model)
        {
            var resultListProduct = await productService.GetAllAsync(model.page, 4, model.searchTerm);
            var resultListCategory = await categoryService.GetAllAsync(model.page, 10, "");
            var totalPages = (int)Math.Ceiling(resultListProduct.Data.TotalItems / (double)resultListProduct.Data.PageSize);
            if (resultListProduct.IsSuccess && resultListCategory.IsSuccess)
            {
                model.Products = resultListProduct.Data.Items;
                model.Categories = resultListCategory.Data.Items;
                model.page = resultListProduct.Data.Page;
                model.pageSize = resultListProduct.Data.PageSize;
                model.totalPages = totalPages;
                model.searchTerm = model.searchTerm ?? "";
                model.categoryId = model.categoryId ?? "";
                return View(model);
            }
            return View();
        }

        [HttpGet("/Product/Detail/{id:guid}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await productService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return View("Notfound");
            }

            var category = await categoryService.GetByIdAsync(result.Data.CategoryId);
            ViewBag.CategoryName = category.Data.Name ?? "Unknown";

            return View(result.Data);
        }
    }
}
