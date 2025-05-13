using EventBus;
using MediatR;
using Order.Application.Interfaces;
using Order.Domain.Agreegates;
using Order.Domain.Events.Domain;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands.Create
{
    public class CreateOrderCommand : IRequest
    {
        public Guid OrderId { get; init; }
        public string CustomerId { get; init; } = default!;
        public List<OrderItem> Items { get; init; } = new();
    }
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public CreateOrderHandler(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var aggregate = OrderAggregate.Create(request.OrderId, request.CustomerId, request.Items);
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedEvents());
            aggregate.MarkEventsAsCommitted();

            await _eventBus.PublishAsync(
                new OrderCreatedDomainEvent(
                    aggregate.Id.ToString(),
                    aggregate.CustomerId,
                    aggregate.Items.Sum(c => c.Quantity * c.Price),
                    DateTime.UtcNow), cancellationToken);
        }
    }
}
