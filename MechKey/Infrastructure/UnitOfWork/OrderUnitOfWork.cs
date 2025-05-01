using Application.Interfaces.IUnitOfWork;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.UnitOfWork
{
    public class OrderUnitOfWork : IOrderUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
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
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }
    }
}
