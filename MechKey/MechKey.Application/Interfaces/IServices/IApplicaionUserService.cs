using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Auth;

namespace Application.Interfaces.IServices
{
    public interface IApplicaionUserService
    {
        Task<Result<ApplicationUserModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<ApplicationUserModel>> AddAsync(RegisterModel model, CancellationToken cancellationToken = default);
        Task<Result<ApplicationUserModel>> UpdateAsync(ApplicationUserModel user, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<ApplicationUserModel>>> GetAllAsync(int page = 1, int pageSize = 10, string searchTerm = "", CancellationToken cancellationToken = default);
        Task UpdateEmailConfirmAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> CheckEmailAddressExists(string email, string phone, CancellationToken cancellationToken = default);
    }
}
