using Microsoft.AspNetCore.Mvc;

namespace MechkeyShop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
