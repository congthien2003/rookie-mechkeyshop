using MongoDB.Driver;
using Notification.Application.Common;
using Notification.Domain.IRepository;
using Notification.Infrastructure.Mappers;
using Notification.Infrastructure.MongoDb.Documents;

namespace Notification.Infrastructure.MongoDb.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<NotificationDocument> _collection;
        private readonly INotificationMapper _mapper;
        private readonly IMongoDbContext _dbContext;

        public NotificationRepository(INotificationMapper mapper, IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
            _collection = dbContext.GetCollection<NotificationDocument>("notifications");
            _mapper = mapper;
        }

        public async Task AddAsync(Domain.Entities.Notification notification)
        {
            var document = _mapper.ToDocument(notification);
            await _collection.InsertOneAsync(document);
        }

        public async Task<IEnumerable<Domain.Entities.Notification>> GetByUserIdAsync(string userId)
        {
            var docs = await _collection.Find(x => x.UserId == "6821707e55a7835ec5a12d70").ToListAsync();
            return docs.Select(_mapper.ToEntity);
        }

        public async Task<Domain.Entities.Notification?> GetByIdAsync(string id)
        {
            var doc = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return doc == null ? null : _mapper.ToEntity(doc);
        }

    }

}
