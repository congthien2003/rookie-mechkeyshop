using MongoDB.Driver;
using Order.Application.Common;
using Order.Application.Interfaces;
using Order.Application.Queries.Models;
using Order.Infrastructure.QueryDbContext.Documents;

namespace Order.Infrastructure.ReadOrderService
{
    public class ReadOrderService : IReadOrderService
    {
        private readonly IMongoDbContext mongoDbContext;

        public ReadOrderService(IMongoDbContext mongoDbContext)
        {
            this.mongoDbContext = mongoDbContext;
        }

        public async Task<List<OrderReadModel>> GetAllByIdUserAsync(string userId)
        {
            var orders = await mongoDbContext.GetCollection<OrderDocument>("order").Find(x => x.CustomerId == userId).ToListAsync();

            var result = orders.Select(order => new OrderReadModel()
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            }).ToList();
            return result;
        }

        public async Task<OrderReadModel> GetByIdAsync(string id)
        {

            var order = await mongoDbContext.GetCollection<OrderDocument>("order").Find(x => x.Id == id).FirstOrDefaultAsync();

            var result = new OrderReadModel()
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };
            return result;
        }
    }
}
