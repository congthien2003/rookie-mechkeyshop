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

        public async Task<Product> CreateAsync(Product entity)
        {
            var result = await context.Products.AddAsync(entity);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(Product entity)
        {
            context.Products.Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> UpdateAsync(Product entity)
        {
            context.Products.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
