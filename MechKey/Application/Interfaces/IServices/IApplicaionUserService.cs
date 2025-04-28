using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Auth;

namespace Application.Interfaces.IServices
{
    public interface IApplicaionUserService
    {
        Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id);
        Task<Result<ApplicationUserModel>> AddAsync(RegisterModel model);
        Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<PagedResult<ApplicationUserModel>>> GetAllAsync(int page = 1, int pageSize = 10, string searchTerm = "");
        Task UpdateEmailConfirmAsync(Guid id);
        Task<bool> CheckEmailAddressExists(string email, string phone);


    }
}
