using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRatingRepository : IProductRatingRepository<ProductRating>
    {
        private readonly ApplicationDbContext _context;

        public ProductRatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductRating> CreateAsync(ProductRating entity, CancellationToken cancellationToken)
        {
            _context.ProductRatings.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<ProductRating> UpdateAsync(ProductRating entity, CancellationToken cancellationToken)
        {
            _context.ProductRatings.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task DeleteAsync(ProductRating entity, CancellationToken cancellationToken)
        {
            _context.ProductRatings.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ProductRating> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.ProductRatings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public IQueryable<ProductRating> GetAllAsync()
        {
            // Không await vì IQueryable là deferred execution
            return _context.ProductRatings.AsQueryable();
        }

        public IQueryable<ProductRating> GetListByProduct(Guid idProduct)
        {
            return _context.ProductRatings.Where(pr => pr.ProductId == idProduct).AsQueryable();
        }
    }
}
