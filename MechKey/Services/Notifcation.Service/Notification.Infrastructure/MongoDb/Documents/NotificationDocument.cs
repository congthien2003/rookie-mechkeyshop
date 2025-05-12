using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Notification.Infrastructure.MongoDb.Documents
{
    public class NotificationDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Giúp Mongo hiểu Id là chuỗi nhưng map với ObjectId
        public string Id { get; set; }

        [BsonElement("type")]
        public int Type { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("eventId")]
        public string EventId { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("recipient")]
        public string Recipient { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("sendAt")]
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
    }
}
