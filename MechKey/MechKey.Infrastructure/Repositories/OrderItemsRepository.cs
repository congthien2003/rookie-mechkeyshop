using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderItemsRepository : RepositoryBase<OrderItem>, IOrderItemsRepository
    {
        public OrderItemsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new IQueryable<OrderItem> GetAllAsync()
        {
            return _dbSet
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .AsNoTracking();
        }

        public new async Task<OrderItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == id, cancellationToken);
        }
    }
}
