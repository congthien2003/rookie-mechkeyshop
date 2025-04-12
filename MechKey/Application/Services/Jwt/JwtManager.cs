using Application.Comoon;
using Application.Interfaces.IServices;
using Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtManager : IJwtManager
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtManager> _logger;

    public JwtManager(ILogger<JwtManager> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public string GenerateToken(ApplicationUser user)
    {
        // Create Claim
        var claims = new[]
        {
            new Claim("Id", user.Id.ToString()),
            new Claim("Email", user.Email),
        };

        // Algorithms
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value!)),
            SecurityAlgorithms.HmacSha256Signature);
        _logger.LogInformation(_configuration.GetSection("JwtSettings:Key").Value!);
        var token = new JwtSecurityToken(
            issuer: _configuration.GetSection("JwtSettings:Issuer").Value,
            audience: _configuration.GetSection("JwtSettings:Audience").Value,
            expires: DateTime.Now.AddHours(int.Parse(_configuration.GetSection("JwtSettings:ExpiredTime").Value!)),
            claims: claims,
            signingCredentials: signingCredentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public Result<string> GetUserIdFromToken(string token)
    {
        throw new NotImplementedException();
    }

    public Result<string> GetUserRoleFromToken(string token)
    {
        throw new NotImplementedException();
    }

    public Result ValidateToken(string token)
    {
        throw new NotImplementedException();
    }
}