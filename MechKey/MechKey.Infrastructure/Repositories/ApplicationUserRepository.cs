using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IApplicationUserRepository<ApplicationUser>
    {
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public bool CheckPhoneExists(string phone)
        {
            return _dbSet.Any(x => x.Phones == phone);
        }

        public new IQueryable<ApplicationUser> GetAllAsync()
        {
            return _dbSet.Where(e => !e.IsDeleted).AsNoTracking();
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public new async Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
