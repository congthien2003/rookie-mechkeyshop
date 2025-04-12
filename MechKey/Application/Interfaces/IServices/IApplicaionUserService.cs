using Application.Comoon;
using Shared.ViewModels;
using Shared.ViewModels.Auth;

namespace Application.Interfaces.IServices
{
    public interface IApplicaionUserService
    {
        Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id);
        Task<Result<ApplicationUserModel>> AddAsync(RegisterModel model);
        Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user);
        Task<Result<ApplicationUserModel>> DeleteAsync(Guid id);

    }
}
