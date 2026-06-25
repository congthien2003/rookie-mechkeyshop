using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new IQueryable<Order> GetAllAsync()
        {
            return _dbSet.Where(e => !e.IsDeleted)
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .AsNoTracking();
        }

        public new async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, cancellationToken);
        }

        public IQueryable<Order> GetOrdersByUserIdAsync(Guid userId)
        {
            return _dbSet.Where(e => !e.IsDeleted)
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .AsQueryable();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted, cancellationToken);
        }
    }
}
