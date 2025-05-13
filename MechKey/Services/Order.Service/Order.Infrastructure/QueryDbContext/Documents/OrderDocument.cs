using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Infrastructure.QueryDbContext.Documents
{
    public class OrderDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("customerId")]
        public string CustomerId { get; set; }
        [BsonElement("status")]
        public string Status { get; set; }
        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; }
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
