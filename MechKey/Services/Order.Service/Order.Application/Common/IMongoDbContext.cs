using MongoDB.Driver;

namespace Order.Application.Common
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
