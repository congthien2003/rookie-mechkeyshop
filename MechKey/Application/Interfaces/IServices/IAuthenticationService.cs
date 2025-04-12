using Application.Comoon;
using Application.DTOs.Auth;
using Shared.ViewModels;
using Shared.ViewModels.Auth;

namespace Application.Interfaces.IServices
{
    public interface IAuthenticationService
    {
        Task<Result> Register(RegisterModel model);
        Task<Result<ApplicationUserModel>> Login(LoginModel model);

    }
}
