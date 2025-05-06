using Domain.Common;

namespace Domain.IRepositories
{
    public interface IApplicationUserRepository<ApplicationUser> : BaseRepository<ApplicationUser> where ApplicationUser : class
    {
        Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        bool CheckPhoneExists(string phone);

    }
}
