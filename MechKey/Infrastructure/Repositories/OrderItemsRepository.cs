using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;

namespace Infrastructure.Repositories
{
    public class OrderItemsRepository : IOrderItemsRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<OrderItem> CreateAsync(OrderItem entity)
        {
            _context.OrderItems.Add(entity);
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(OrderItem entity)
        {
            _context.OrderItems.Remove(entity);
            return Task.CompletedTask;
        }

        public IQueryable<OrderItem> GetAllAsync()
        {
            return _context.OrderItems
                .AsQueryable();
        }

        public Task<OrderItem> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OrderItem> UpdateAsync(OrderItem entity)
        {
            throw new NotImplementedException();
        }
    }
}