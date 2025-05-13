using Order.Domain.Events;
using Order.Domain.ValueObjects;

namespace Order.Domain.Agreegates
{
    public class OrderAggregate
    {
        public Guid Id { get; private set; }
        public string CustomerId { get; private set; } = default!;
        public List<OrderItem> Items { get; private set; } = new();
        public bool IsPaid { get; private set; }

        private readonly List<object> _uncommittedEvents = new();

        public IEnumerable<object> GetUncommittedEvents() => _uncommittedEvents;
        public void MarkEventsAsCommitted() => _uncommittedEvents.Clear();

        public static OrderAggregate Create(Guid orderId, string customerId, List<OrderItem> items)
        {
            var aggregate = new OrderAggregate();
            var @event = new OrderCreatedEvent(orderId, customerId, items);
            aggregate.Apply(@event);
            aggregate._uncommittedEvents.Add(@event);
            return aggregate;
        }

        public void Pay()
        {
            if (IsPaid)
                throw new InvalidOperationException("Order already paid");

            var @event = new OrderPaidEvent(Id, DateTime.UtcNow);
            Apply(@event);
            _uncommittedEvents.Add(@event);
        }

        #region Apply method
        public void Apply(OrderCreatedEvent @event)
        {
            Id = @event.OrderId;
            CustomerId = @event.CustomerId;
            Items = @event.Items;
        }

        public void Apply(OrderPaidEvent @event)
        {
            IsPaid = true;
        }

        #endregion

    }
}
