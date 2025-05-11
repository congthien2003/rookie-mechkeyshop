using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Auth;

namespace Shared.Mapping.Interfaces
{
    public interface IApplicationUserMapping
    {
        ApplicationUserModel ToApplicationUserModel(ApplicationUser applicationUser);
        List<ApplicationUserModel> ToListApplicationUserModel(List<ApplicationUser> applicationUsers);
        ApplicationUser ToApplicationUser(ApplicationUserModel applicationUser);
    }
}
