using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Order.Application.Common;

namespace Order.Infrastructure.ReadDbContext
{
    public class QueryDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public QueryDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoSettings:ConnectionString"]);
            _database = client.GetDatabase(configuration["MongoSettings:Database"]);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
