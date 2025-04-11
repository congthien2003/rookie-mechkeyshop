using Application.Comoon;
using Domain.Entity;

namespace Application.Interfaces.IServices
{
    public interface IJwtManager
    {
        string GenerateToken(ApplicationUser user);
        Result ValidateToken(string token);
    }
}