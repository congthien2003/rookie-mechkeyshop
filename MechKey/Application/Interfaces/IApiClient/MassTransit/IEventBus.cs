namespace Application.Interfaces.IApiClient.MassTransit
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    }
}
