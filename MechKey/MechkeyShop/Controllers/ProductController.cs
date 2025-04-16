using Application.Interfaces.IServices;
using MechkeyShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels;

namespace MechkeyShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IApplicaionUserService applicaionUserService;
        private readonly IProductRatingService ratingService;

        public ProductController(IProductService productService,
            ICategoryService categoryService,
            IApplicaionUserService applicaionUserService,
            IProductRatingService ratingService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.applicaionUserService = applicaionUserService;
            this.ratingService = ratingService;
        }

        public async Task<IActionResult> IndexAsync(ProductPageViewModel model)
        {
            var ascOrder = model.AscendingOrder == 1 ? true : false;
            var resultListProduct = await productService.GetAllAsync(model.Page, model.PageSize, model.CategoryId, model.SearchTerm, model.SortCol, ascOrder);
            var resultListCategory = await categoryService.GetAllAsync(model.Page, 10, "");
            var totalPages = (int)Math.Ceiling(resultListProduct.Data.TotalItems / (double)resultListProduct.Data.PageSize);
            if (resultListProduct.IsSuccess && resultListCategory.IsSuccess)
            {

                if (model.CategoryId is not null)
                {
                    ViewBag.Title = resultListCategory.Data.Items.FirstOrDefault(c => c.Id.ToString() == model.CategoryId)?.Name ?? "Product List";
                }

                model.Products = resultListProduct.Data.Items;
                model.Categories = resultListCategory.Data.Items;
                model.Page = resultListProduct.Data.Page;
                model.PageSize = resultListProduct.Data.PageSize;
                model.TotalPages = totalPages;
                model.SearchTerm = model.SearchTerm ?? "";
                model.CategoryId = model.CategoryId ?? "";
                return View(model);
            }
            return View();
        }

        [HttpGet("/Product/Detail/{id:guid}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await productService.GetByIdAsync(id);

            var category = await categoryService.GetByIdAsync(result.Data.CategoryId);

            var productRating = await ratingService.GetAllByIdProductAsync(id, 4, false);

            if (productRating.IsSuccess && productRating.Data.Items.Count() > 0)
            {
                result.Data.Rating = productRating.Data.Items;
                result.Data.TotalRating = productRating.Data.Items.Average(p => p.Stars);
            }
            return View(result.Data);
        }

        [HttpPost("/Product/SubmitRating/{id:guid}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> SubmitRating(Guid id, int score, string comment)
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst("Id")?.Value;

                var infoUser = await applicaionUserService.GetByIdAsync(Guid.Parse(userId));

                var model = new ProductRatingViewModel
                {
                    Stars = score,
                    Comment = comment ?? "",
                    UserId = Guid.Parse(userId),
                    ProductId = id,
                    RatedAt = DateTime.UtcNow,
                };

                var result = await ratingService.AddAsync(model);

                return Redirect($"/Product/Detail/{id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Detail", "Product", id);
            }
        }
    }
}
