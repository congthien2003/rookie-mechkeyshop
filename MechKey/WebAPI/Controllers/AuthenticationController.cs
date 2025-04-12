using Application.Comoon;
using Application.Interfaces.IServices;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels.Auth;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IJwtManager _jwtManager;
        private readonly IApplicaionUserService applicaionUserService;

        public AuthenticationController(IJwtManager jwtManager,
            ILogger<AuthenticationController> logger,
            IApplicaionUserService applicaionUserService)
        {
            _jwtManager = jwtManager;
            _logger = logger;
            this.applicaionUserService = applicaionUserService;
        }

        [HttpGet]
        public Result<string> GetToken()
        {
            _logger.LogInformation("Get token");
            var user = new ApplicationUser
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
        public ActionResult<Result> ValidateToken()
        {
            _logger.LogInformation("Validate token");
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")[1];
            if (string.IsNullOrEmpty(token))
                return BadRequest(Result.Failure("Token is missing"));
            var result = _jwtManager.ValidateToken(token);
            return Result.Success("Valid token");
        }

        [HttpPost]
        public async Task<Result> Register(RegisterModel model)
        {
            if (model is null)
            {
                return Result.Failure("Model is null");
            }
            await applicaionUserService.AddAsync(model);

            return Result.Success("Register Success");
        }
    }
}