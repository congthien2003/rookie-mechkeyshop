using Application.Comoon;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces.IServices
{
    public interface IJwtManager
    {
        string GenerateToken(IdentityUser user);
        Result ValidateToken(string token);
        Result<string> GetUserIdFromToken(string token);
        Result<string> GetUserRoleFromToken(string token);
    }
}