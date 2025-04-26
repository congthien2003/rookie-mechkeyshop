using Application.Interfaces.IApiClient.MassTransit;
using MassTransit;

namespace Infrastructure.ApiClient
{
    public class MassTransitService : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            await _publishEndpoint.Publish(@event);
        }
    }
}
