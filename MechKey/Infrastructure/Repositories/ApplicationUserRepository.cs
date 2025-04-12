using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            var user = await context.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == email);
            return user;
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
