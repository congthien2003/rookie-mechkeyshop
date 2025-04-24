using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;

namespace Infrastructure.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ProductImage> CreateAsync(ProductImage entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.ProductImages.Add(entity);
            return Task.FromResult(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = _context.ProductImages.FirstOrDefault(u => u.Id == id);
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.ProductImages.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ProductImage> GetAllAsync()
        {
            return _context.ProductImages.AsQueryable();
        }

        public async Task<ProductImage> GetByIdAsync(Guid id)
        {
            return await _context.ProductImages.FindAsync(id);
        }
    }
}
