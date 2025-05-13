using Order.Domain.ValueObjects;

namespace Order.Domain.Events
{
    public record OrderCreatedEvent(
        Guid OrderId,
        string CustomerId,
        List<OrderItem> Items
    ) : OrderEvent(OrderId, DateTime.UtcNow);
}
