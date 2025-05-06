using Application.Comoon;
using Shared.ViewModels.Auth;

namespace Application.Interfaces.IServices
{
    public interface IAuthenticationService
    {
        Task<Result> Register(RegisterModel model, CancellationToken cancellationToken = default);
        Task<Result<ApplicationUserModel>> Login(LoginModel model, CancellationToken cancellationToken = default);

    }
}
