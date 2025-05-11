namespace Constracts.Events
{
    public abstract class BaseEvent
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
