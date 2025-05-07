using Application.Interfaces.IServices;
using Domain.Common;
using MechkeyShop.Constant;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels.Auth;

namespace MechkeyShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly IJwtManager _jwtManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IApplicaionUserService _applicaionUserService;

        public AuthController(
            IJwtManager jwtManager,
            IAuthenticationService authenticationService,
            IApplicaionUserService applicaionUserService)
        {
            _jwtManager = jwtManager;
            _authenticationService = authenticationService;
            _applicaionUserService = applicaionUserService;
        }

        // GET: AuthController
        public ActionResult Index()
        {
            return View();
        }

        // Get Login Page
        // Auth/Login
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // Post Login
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginModel model, string returnUrl)
        {
            returnUrl = returnUrl ?? "/";
            try
            {
                var result = await _authenticationService.Login(model);

                var token = _jwtManager.GenerateToken(result.Data);

                // Add Token into Cookies
                Response.Cookies.Append("accessToken", token, new CookieOptions
                {
                    HttpOnly = true,  // Prevent JavaScript access
                    Secure = true,    // Only send over HTTPS
                    Expires = DateTime.UtcNow.AddMinutes(15) // Expiration time
                });

                Response.Cookies.Append("username", result.Data.Name.ToString(), new CookieOptions
                {
                    HttpOnly = true,  // Prevent JavaScript access
                    Secure = true,    // Only send over HTTPS
                    Expires = DateTime.UtcNow.AddMinutes(15) // Expiration time
                });

                // show toast
                TempData[Toast.KEY] = "Login success";
                TempData[Toast.MESSAGE] = "Shopping now!";
                TempData[Toast.TYPE] = Toast.SUCCESS_TYPE;

                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");

            }
            catch (BaseException ex)
            {
                //_logger.LogError(ex.Message);
                ViewBag.Error = ex.Message;
                TempData[Toast.KEY] = "Login failed";
                TempData[Toast.MESSAGE] = ex.Message;
                TempData[Toast.TYPE] = Toast.ERROR_TYPE;
                ViewBag.returnUrl = returnUrl;
                return View(model);

            }

        }

        // Get Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[Toast.KEY] = "Register failed";
                TempData[Toast.MESSAGE] = "Invalid Register Data";
                TempData[Toast.TYPE] = Toast.ERROR_TYPE;
                return View(model);
            }
            try
            {
                var result = await _authenticationService.Register(model);
                return View("RegisterSuccess");
            }
            catch (BaseException ex)
            {
                ViewBag.Error = "Register failed";
                TempData[Toast.KEY] = "Failed";
                TempData[Toast.MESSAGE] = ex.Message;
                TempData[Toast.TYPE] = Toast.ERROR_TYPE;
                return View("Register");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("accessToken");

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [HttpGet("confirm/{id:guid}")]
        public async Task<IActionResult> ConfirmEmailAsync(Guid id)
        {
            // Change IsConfirmed user
            await _applicaionUserService.UpdateEmailConfirmAsync(id);
            // -> Redirect to login page
            return RedirectToAction("Login", "Auth");
        }

    }
}
