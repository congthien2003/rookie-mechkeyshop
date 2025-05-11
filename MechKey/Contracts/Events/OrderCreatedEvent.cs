
using Shared.ViewModels.Order;
namespace Constracts.Events
{
    public class OrderCreatedEvent : BaseEvent
    {
        public OrderModel OrderModel { get; set; }
    }
}
