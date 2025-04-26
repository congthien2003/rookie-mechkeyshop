namespace Application.Events
{
    public class OrderCreatedEvent : BaseEvent
    {
        public Guid OrderId { get; set; }
    }
}
