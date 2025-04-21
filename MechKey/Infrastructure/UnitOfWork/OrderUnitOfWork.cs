using Application.Interfaces.IUnitOfWork;
using Domain.IRepositories;
using MechkeyShop.Data;

namespace Infrastructure.UnitOfWork
{
    public class OrderUnitOfWork : IOrderUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IOrderRepository Orders { get; }

        public IOrderItemsRepository OrderItems { get; }

        public OrderUnitOfWork(IOrderRepository orders,
                               IOrderItemsRepository orderItems,
                               ApplicationDbContext context)
        {
            Orders = orders;
            OrderItems = orderItems;
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
