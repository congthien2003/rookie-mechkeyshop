using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels;

namespace MechkeyShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly IJwtManager _jwtManager;

        public AuthController(IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

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
            var user = new ApplicationUserModel
            {
                Id = Guid.NewGuid(),
                Email = "abc@example.com",
            };

            var token = _jwtManager.GenerateToken(user);

            // Add Token into Cookies
            Response.Cookies.Append("accessToken", token, new CookieOptions
            {
                HttpOnly = true,  // Prevent JavaScript access
                Secure = true,    // Only send over HTTPS
                Expires = DateTime.UtcNow.AddMinutes(15) // Expiration time
            });
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
