using MongoDB.Driver;

namespace Notification.Application.Common
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
