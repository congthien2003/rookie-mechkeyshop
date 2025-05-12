using Notification.Domain.Enum;
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
                Type = (int)notification.Type,
                Description = notification.Description,
                EventId = notification.EventId,
                UserId = notification.UserId,
                Recipient = notification.Recipient,
                Status = (int)notification.Status,
                SendAt = notification.SendAt
            };
        }

        Domain.Entities.Notification INotificationMapper.ToEntity(NotificationDocument document)
        {
            return new Domain.Entities.Notification
            {
                Id = document.Id,
                Type = document.Type == (int)NotificationType.Email ? NotificationType.Email : NotificationType.SMS,
                Description = document.Description,
                EventId = document.EventId,
                UserId = document.UserId,
                Recipient = document.Recipient,
                Status = document.Status == (int)NotificationStatus.Failed ? NotificationStatus.Failed : NotificationStatus.Success,
                SendAt = document.SendAt
            };
        }
    }

}
