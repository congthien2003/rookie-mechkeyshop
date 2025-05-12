namespace EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class;
    }
}
