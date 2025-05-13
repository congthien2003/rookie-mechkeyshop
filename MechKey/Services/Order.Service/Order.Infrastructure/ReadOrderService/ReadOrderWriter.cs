using MongoDB.Driver;
using Order.Application.Common;
using Order.Application.Interfaces;
using Order.Application.Queries.Models;
using Order.Infrastructure.QueryDbContext.Documents;

namespace Order.Infrastructure.ReadOrderService
{
    public class ReadOrderWriter : IReadOrderWriter
    {
        private readonly IMongoDbContext mongoDbContext;

        public ReadOrderWriter(IMongoDbContext mongoDbContext)
        {
            this.mongoDbContext = mongoDbContext;
        }

        public async Task InsertAsync(OrderReadModel model)
        {
            var document = new OrderDocument
            {
                Id = model.Id,
                CustomerId = model.CustomerId,
                CreatedAt = model.CreatedAt,
                Status = model.Status,
                TotalAmount = model.TotalAmount
            };

            await mongoDbContext.GetCollection<OrderDocument>("order").InsertOneAsync(document);
        }

        public async Task UpdateStatusAsync(string orderId, string newStatus)
        {
            var collection = mongoDbContext.GetCollection<OrderDocument>("order");
            var filter = Builders<OrderDocument>.Filter.Eq(x => x.Id, orderId);
            var update = Builders<OrderDocument>.Update.Set(x => x.Status, newStatus);

            await collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string orderId)
        {
            var collection = mongoDbContext.GetCollection<OrderDocument>("order");
            var filter = Builders<OrderDocument>.Filter.Eq(x => x.Id, orderId);

            await collection.DeleteOneAsync(filter);
        }
    }
}
