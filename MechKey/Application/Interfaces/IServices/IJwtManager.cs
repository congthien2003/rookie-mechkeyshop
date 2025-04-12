using Application.Comoon;
using Shared.ViewModels;

namespace Application.Interfaces.IServices
{
    public interface IJwtManager
    {
        string GenerateToken(ApplicationUserModel user);
        Result ValidateToken(string token);
    }
}