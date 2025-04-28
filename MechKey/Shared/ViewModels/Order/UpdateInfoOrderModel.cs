namespace Shared.ViewModels.Order
{
    public class UpdateInfoOrderModel
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
