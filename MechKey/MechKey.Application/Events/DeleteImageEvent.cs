namespace Application.Events
{
    public class DeleteImageEvent : BaseEvent
    {
        public string Url { get; set; }
    }
}
