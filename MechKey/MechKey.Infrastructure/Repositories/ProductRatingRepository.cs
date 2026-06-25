using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRatingRepository : RepositoryBase<ProductRating>, IProductRatingRepository<ProductRating>
    {
        public ProductRatingRepository(ApplicationDbContext context) : base(context)
        {
        }

        // ProductRating uses int Id, not Guid, so this is the only override needed
        public new async Task<ProductRating> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw new InvalidOperationException($"ProductRating with id {id} not found.");
        }

        public new IQueryable<ProductRating> GetAllAsync()
        {
            return _dbSet.AsQueryable();
        }

        // ProductRating does hard delete, not soft delete
        public new async Task DeleteAsync(ProductRating entity, CancellationToken cancellationToken)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<ProductRating> GetListByProduct(Guid idProduct)
        {
            return _dbSet.Where(pr => pr.ProductId == idProduct).AsQueryable();
        }
    }
}
