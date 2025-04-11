using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;

namespace Infrastructure.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository<ApplicationUser>
    {
        private readonly ApplicationDbContext context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<ApplicationUser> CreateAsync(ApplicationUser entity)
        {
            var newUser = context.ApplicationUsers.Add(entity);
            await context.SaveChangesAsync();
            return newUser.Entity;
        }

        public Task DeleteAsync(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
