namespace Order.Application.Interfaces
{
    public interface IEventStore
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<object> events);
        Task<List<object>> GetEventsAsync(Guid aggregateId);
    }
}
