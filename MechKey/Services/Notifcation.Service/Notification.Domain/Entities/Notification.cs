namespace Notification.Domain.Entities
{

    public class Notification
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public string EventId { get; set; }
        public string UserId { get; set; }
        // Email or SMS
        public string Recipient { get; set; }
        public int Status { get; set; } // Success / Failed
        public DateTime SendAt { get; set; } = DateTime.UtcNow;

    }
}
