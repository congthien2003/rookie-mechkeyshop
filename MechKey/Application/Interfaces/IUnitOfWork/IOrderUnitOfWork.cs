using Domain.IRepositories;

namespace Application.Interfaces.IUnitOfWork
{
    public interface IOrderUnitOfWork : BaseUnitOfWork
    {
        IOrderRepository Orders { get; }
        IOrderItemsRepository OrderItems { get; }

    }
}
