namespace Shared.ViewModels.Abstractions
{
    public class BaseViewModel
    {
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public Guid UpdateById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
