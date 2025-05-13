namespace Order.Domain.Events
{
    public record OrderPaidEvent(Guid OrderId, DateTime PaidAt) : OrderEvent(OrderId, DateTime.UtcNow);
}
