namespace EventBus
{
    public interface IEventBus
    {
        Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class;
    }
}
