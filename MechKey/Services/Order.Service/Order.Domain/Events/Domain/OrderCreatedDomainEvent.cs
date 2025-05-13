namespace Order.Domain.Events.Domain
{
    public class OrderCreatedDomainEvent
    {
        public string OrderId { get; }
        public string CustomerId { get; }
        public decimal TotalAmount { get; }
        public DateTime CreatedAt { get; }

        public OrderCreatedDomainEvent(string orderId, string customerId, decimal totalAmount, DateTime createdAt)
        {
            OrderId = orderId;
            CustomerId = customerId;
            TotalAmount = totalAmount;
            CreatedAt = createdAt;
        }
    }
}
