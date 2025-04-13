using Application.Comoon;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels;
using Shared.ViewModels.Auth;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IJwtManager _jwtManager;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IJwtManager jwtManager,
            ILogger<AuthenticationController> logger,
            IAuthenticationService authenticationService)
        {
            _jwtManager = jwtManager;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public Result<string> GetToken()
        {
            _logger.LogInformation("Get token");
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
            return Result<string>.Success("Get Token Success", token);
        }

        [HttpGet("/test-api")]
        [Authorize]
        public IActionResult TestAPI()
        {
            return Ok();
        }

        [HttpPost("validate")]
        [Authorize]
        public ActionResult<Result> ValidateToken()
        {
            _logger.LogInformation("Validate token");
            return Ok();
        }

        [HttpPost("register")]
        public async Task<Result> Register(RegisterModel model)
        {
            if (model is null)
            {
                return Result.Failure("Model is null");
            }
            return await _authenticationService.Register(model);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result>> Login(LoginModel model)
        {
            if (model is null)
            {
                throw new InvalidDataException("Missing login data");
            }
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
                return Result.Success("Login Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(Result.Failure("Login Failed"));
            }
        }
    }
}