using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechkeyShop.Controllers
{
    public class AuthController : Controller
    {
        // GET: AuthController

        public ActionResult Index()
        {
            return View();
        }

        // Get Login Page
        // Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Post Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            return RedirectToAction("Index");
        }

        // Get Register Page
        [HttpGet]
        [Authorize]
        public IActionResult Register()
        {
            return View();
        }
    }
}
