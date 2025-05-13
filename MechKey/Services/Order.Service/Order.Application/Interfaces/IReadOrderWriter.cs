using Order.Application.Queries.Models;

namespace Order.Application.Interfaces
{
    public interface IReadOrderWriter
    {
        Task InsertAsync(OrderReadModel model);
        Task UpdateStatusAsync(string orderId, string newStatus);
        Task DeleteAsync(string orderId);
    }
}
