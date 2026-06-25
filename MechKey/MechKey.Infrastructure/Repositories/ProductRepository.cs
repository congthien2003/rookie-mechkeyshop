using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository<Product>
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new IQueryable<Product> GetAllAsync()
        {
            return _dbSet.Where(e => !e.IsDeleted)
                .Include(p => p.Category)
                .Include(p => p.ProductRatings)
                .AsNoTracking();
        }

        public new async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductRatings)
                .ThenInclude(pr => pr.User)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
        }
    }
}
