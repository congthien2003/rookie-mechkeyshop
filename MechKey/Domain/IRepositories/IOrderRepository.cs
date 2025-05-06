using Domain.Common;
using Domain.Entity;

namespace Domain.IRepositories
{
    public interface IOrderRepository : BaseRepository<Order>
    {
        Task<Order> GetOrderWithDetailsAsync(Guid orderId, CancellationToken cancellationToken);
        IQueryable<Order> GetOrdersByUserIdAsync(Guid userId);
    }
}