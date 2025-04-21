using Domain.IRepositories;

namespace Application.Interfaces.IUnitOfWork
{
    public interface IOrderUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        IOrderItemsRepository OrderItems { get; }
        Task<int> SaveChangesAsync();
    }
}
