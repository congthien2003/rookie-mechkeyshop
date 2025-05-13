using MediatR;
using Order.Application.Interfaces;
using Order.Domain.Agreegates;

namespace Order.Application.Commands.Pay
{
    public class PayOrderCommand : IRequest
    {
        public Guid OrderId { get; init; }
    }

    public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand>
    {
        private readonly IEventStore _eventStore;

        public PayOrderCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventStore.GetEventsAsync(request.OrderId);
            if (!events.Any())
                throw new InvalidOperationException("Order not found");

            var aggregate = new OrderAggregate();
            foreach (var evt in events)
            {
                // sử dụng dynamic dispatch hoặc pattern matching để gọi Apply tương ứng
                switch (evt)
                {
                    case Domain.Events.OrderCreatedEvent oce:
                        aggregate.Apply(oce); break;
                    case Domain.Events.OrderPaidEvent ope:
                        aggregate.Apply(ope); break;
                }
            }

            aggregate.Pay();
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedEvents());
            aggregate.MarkEventsAsCommitted();
        }
    }
}
