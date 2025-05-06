using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Order> CreateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            var result = await context.Orders.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task DeleteAsync(Order entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            context.Orders.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<Order> GetAllAsync()
        {
            return context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public IQueryable<Order> GetOrdersByUserIdAsync(Guid userId)
        {
            return context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public async Task<Order> UpdateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            context.Orders.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}