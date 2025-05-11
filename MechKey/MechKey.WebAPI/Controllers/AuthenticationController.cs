using Application.Comoon;
using Application.Interfaces.IServices;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.ViewModels.Auth;
namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/auth")]
    [ApiController]

    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IJwtManager _jwtManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        public AuthenticationController(IJwtManager jwtManager,
            ILogger<AuthenticationController> logger,
            IAuthenticationService authenticationService,
            IMapper mapper)
        {
            _jwtManager = jwtManager;
            _logger = logger;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<Result> Register(RegisterModel model, CancellationToken cancellationToken = default)
        {
            if (model is null)
            {
                return Result.Failure("Model is null");
            }
            return await _authenticationService.Register(model, cancellationToken);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result>> Login(LoginModel model, CancellationToken cancellationToken = default)
        {
            if (model is null)
            {
                throw new InvalidDataException("Missing login data");
            }
            var result = await _authenticationService.Login(model, cancellationToken);

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
                return Ok(Result<ApplicationUserModel>.Success("Login Success", _mapper.Map<ApplicationUserModel>(result.Data)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(Result.Failure("Login Failed"));
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<Result>> LogOut(CancellationToken cancellationToken = default)
        {
            Response.Cookies.Delete("accessToken");
            return Ok(Result.Success("Logout Success"));
        }
    }
}