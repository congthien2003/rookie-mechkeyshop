namespace Order.Domain.Events
{
    public abstract record OrderEvent(Guid OrderId, DateTime TimeStamp)
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public long Version { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

}
