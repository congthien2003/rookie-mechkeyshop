using Shared.ViewModels.Order;

namespace Application.Events
{
    public class OrderCreatedEvent : BaseEvent
    {
        public OrderModel OrderModel { get; set; }
    }
}
