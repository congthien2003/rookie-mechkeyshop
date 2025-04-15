using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;

namespace Infrastructure.Repositories
{
    public class ProductRatingRepository : IProductRatingRepository<ProductRating>
    {
        private readonly ApplicationDbContext _context;

        public ProductRatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductRating> CreateAsync(ProductRating entity)
        {
            _context.ProductRatings.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ProductRating> UpdateAsync(ProductRating entity)
        {
            _context.ProductRatings.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(ProductRating entity)
        {
            _context.ProductRatings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductRating> GetByIdAsync(Guid id)
        {
            return await _context.ProductRatings.FindAsync(id);
        }

        public IQueryable<ProductRating> GetAllAsync()
        {
            // Không await vì IQueryable là deferred execution
            return _context.ProductRatings.AsQueryable();
        }
    }
}
