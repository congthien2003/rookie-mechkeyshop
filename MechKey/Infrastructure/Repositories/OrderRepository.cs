using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Order> CreateAsync(Order entity)
        {
            _context.Orders.Add(entity);
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(Order entity)
        {
            _context.Orders.Remove(entity);
            return Task.CompletedTask;
        }

        public IQueryable<Order> GetAllAsync()
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();
        }

        public Task<Order?> GetByIdAsync(Guid id)
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public IQueryable<Order> GetOrdersByUserIdAsync(Guid userId)
        {
            return _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public Task<Order> UpdateAsync(Order entity)
        {
            _context.Orders.Update(entity);
            return Task.FromResult(entity);
        }
    }
}