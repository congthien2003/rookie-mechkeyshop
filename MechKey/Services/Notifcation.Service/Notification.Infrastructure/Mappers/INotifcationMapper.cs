using Notification.Infrastructure.MongoDb.Documents;

namespace Notification.Infrastructure.Mappers
{
    public interface INotificationMapper
    {
        NotificationDocument ToDocument(Domain.Entities.Notification notification);
        Domain.Entities.Notification ToEntity(NotificationDocument document);
    }
}
