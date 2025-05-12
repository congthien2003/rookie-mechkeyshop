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

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class
        {
            await _publisher.Publish(@event, cancellationToken);
        }
    }
}
