using Application.Interfaces.IServices;
using MechkeyShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MechkeyShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        public HomeController(ILogger<HomeController> logger,
            IProductService productService,
            ICategoryService categoryService)
        {
            _logger = logger;
            this.productService = productService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var resultListProduct = await productService.GetAllAsync(1, 10, "");
            var resultListCategory = await categoryService.GetAllAsync(1, 10, "");
            var resultListBestSeller = await productService.GetBestSellerAsync();
            if (resultListProduct.IsSuccess)
            {
                ViewBag.listCategory = resultListCategory.Data.Items;
                ViewBag.listBestSeller = resultListBestSeller.Data;
                return View(resultListProduct.Data.Items);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("/access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
