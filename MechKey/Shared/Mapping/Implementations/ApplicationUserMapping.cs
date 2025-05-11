using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Auth;

namespace Shared.Mapping.Implementations
{
    public class ApplicationUserMapping : IApplicationUserMapping
    {
        public ApplicationUserModel ToApplicationUserModel(ApplicationUser applicationUser)
        {
            return new ApplicationUserModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                Name = applicationUser.Name,
                IsDeleted = applicationUser.IsDeleted,
                Address = applicationUser.Address ?? "",
                Phones = applicationUser.Phones,
                RoleId = applicationUser.RoleId,
                CreatedAt = applicationUser.CreatedAt,
                LastUpdatedAt = applicationUser.LastUpdatedAt ?? null,
                UpdateById = applicationUser.UpdateById
            };
        }

        public List<ApplicationUserModel> ToListApplicationUserModel(List<ApplicationUser> applicationUsers)
        {
            var data = new List<ApplicationUserModel>();

            foreach (var item in applicationUsers)
            {
                data.Add(ToApplicationUserModel(item));
            }

            return data;
        }

        public ApplicationUser ToApplicationUser(ApplicationUserModel applicationUser)
        {
            return new ApplicationUser
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                Name = applicationUser.Name,
                IsDeleted = applicationUser.IsDeleted,
                Address = applicationUser.Address ?? "",
                Phones = applicationUser.Phones,
                RoleId = applicationUser.RoleId,
                CreatedAt = applicationUser.CreatedAt,
                LastUpdatedAt = applicationUser.LastUpdatedAt ?? null,
                UpdateById = applicationUser.UpdateById

            };
        }
    }
}
