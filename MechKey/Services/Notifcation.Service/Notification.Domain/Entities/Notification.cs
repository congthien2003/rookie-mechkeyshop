using Notification.Domain.Enum;

namespace Notification.Domain.Entities
{

    public class Notification
    {
        public string Id { get; set; }
        public NotificationType Type { get; set; } // Email or SMS
        public string Description { get; set; }
        public string EventId { get; set; }
        public string UserId { get; set; }
        public string Recipient { get; set; } // Email or SMS
        public NotificationStatus Status { get; set; } // Success / Failed
        public DateTime SendAt { get; set; } = DateTime.UtcNow;

        public void ChangeStatus(NotificationStatus status)
        {
            switch (status)
            {
                case NotificationStatus.Failed:
                    Status = NotificationStatus.Failed;
                    break;
                case NotificationStatus.Success:
                    Status = NotificationStatus.Success;
                    break;
            }
        }


    }
}
