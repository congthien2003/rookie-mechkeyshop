using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository<Product>
    {
        private readonly ApplicationDbContext context;

        public ProductRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Product> CreateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            var result = await context.Products.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task DeleteAsync(Product entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            context.Products.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<Product> GetAllAsync()
        {
            return context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductRatings)
                .AsQueryable();
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductRatings)
                .ThenInclude(pr => pr.User)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product> UpdateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            context.Products.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}
