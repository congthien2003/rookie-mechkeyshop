namespace Shared.ViewModels
{
    public class CreateOrderModel
    {
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public double TotalAmount { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public IEnumerable<CreateOrderItemViewModel> OrderItems { get; set; } = Enumerable.Empty<CreateOrderItemViewModel>();

    }

    public class CreateOrderItemViewModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public string Option { get; set; } = string.Empty;
    }
}
