using MassTransit;

namespace EventBus.Implementation
{
    public class EventBus : IEventBus
    {
        private readonly IPublishEndpoint _publisher;

        public EventBus(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class
        {
            _publisher.Publish(@event, cancellationToken);
            return Task.CompletedTask;
        }
    }
}
