namespace Order.Application.Queries.Models
{
    public class OrderReadModel
    {
        public string Id { get; set; }
        public string CustomerId { get; set; } = default!;
        public string Status { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
