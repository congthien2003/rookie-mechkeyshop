using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entity.Name))
                throw new CategoryValidateFailedException();
            return await base.CreateAsync(entity, cancellationToken);
        }

        public new IQueryable<Category> GetAllAsync()
        {
            return _dbSet.Where(e => !e.IsDeleted).Include(c => c.Products).AsNoTracking();
        }

        public new async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }
    }
}
