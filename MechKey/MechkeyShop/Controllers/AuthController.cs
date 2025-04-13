using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels.Auth;

namespace MechkeyShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly IJwtManager _jwtManager;
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IJwtManager jwtManager, IAuthenticationService authenticationService)
        {
            _jwtManager = jwtManager;
            _authenticationService = authenticationService;
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
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authenticationService.Login(model);

            try
            {
                var token = _jwtManager.GenerateToken(result.Data);

                // Add Token into Cookies
                Response.Cookies.Append("accessToken", token, new CookieOptions
                {
                    HttpOnly = true,  // Prevent JavaScript access
                    Secure = true,    // Only send over HTTPS
                    Expires = DateTime.UtcNow.AddMinutes(15) // Expiration time
                });
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);
                return RedirectToAction("/access-denied");

            }
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
