using Notification.Infrastructure.MongoDb.Documents;

namespace Notification.Infrastructure.Mappers
{
    public class NotificationMapper : INotificationMapper
    {
        public NotificationDocument ToDocument(Domain.Entities.Notification notification)
        {
            return new NotificationDocument
            {
                Id = notification.Id,
                Type = notification.Type,
                Description = notification.Description,
                EventId = notification.EventId,
                UserId = notification.UserId,
                Recipient = notification.Recipient,
                Status = notification.Status,
                SendAt = notification.SendAt
            };
        }

        Domain.Entities.Notification INotificationMapper.ToEntity(NotificationDocument document)
        {
            return new Domain.Entities.Notification
            {
                Id = document.Id,
                Type = document.Type,
                Description = document.Description,
                EventId = document.EventId,
                UserId = document.UserId,
                Recipient = document.Recipient,
                Status = document.Status,
                SendAt = document.SendAt
            };
        }
    }

}
