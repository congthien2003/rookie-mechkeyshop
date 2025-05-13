using Order.Application.Queries.Models;

namespace Order.Application.Interfaces
{
    public interface IReadOrderService
    {
        public Task<OrderReadModel> GetByIdAsync(string id);

        public Task<List<OrderReadModel>> GetAllByIdUserAsync(string userId);
    }
}
