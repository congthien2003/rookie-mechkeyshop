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

        public async Task DeleteAsync(ApplicationUser entity)
        {
            entity.IsDeleted = true;
            context.ApplicationUsers.Update(entity);
            await context.SaveChangesAsync();
        }

        public IQueryable<ApplicationUser> GetAllAsync()
        {
            return context.ApplicationUsers.AsQueryable();
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            var user = await context.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid id)
        {
            return await context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
        {
            context.ApplicationUsers.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
