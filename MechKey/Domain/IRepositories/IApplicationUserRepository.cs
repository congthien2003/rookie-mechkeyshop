using Domain.Common;

namespace Domain.IRepositories
{
    public interface IApplicationUserRepository<ApplicationUser> : BaseRepository<ApplicationUser> where ApplicationUser : class
    {

    }
}
