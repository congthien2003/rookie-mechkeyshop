namespace Application.Events
{
    public class RegisterSuccessEvent : BaseEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }

        public RegisterSuccessEvent() : base()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
