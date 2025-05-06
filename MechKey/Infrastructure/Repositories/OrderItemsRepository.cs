using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderItemsRepository : IOrderItemsRepository
    {
        private readonly ApplicationDbContext context;

        public OrderItemsRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<OrderItem> CreateAsync(OrderItem entity, CancellationToken cancellationToken = default)
        {
            var result = await context.OrderItems.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }

        public async Task DeleteAsync(OrderItem entity, CancellationToken cancellationToken = default)
        {
            context.OrderItems.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<OrderItem> GetAllAsync()
        {
            return context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .AsQueryable();
        }

        public async Task<OrderItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == id, cancellationToken);
        }

        public async Task<OrderItem> UpdateAsync(OrderItem entity, CancellationToken cancellationToken = default)
        {
            context.OrderItems.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}