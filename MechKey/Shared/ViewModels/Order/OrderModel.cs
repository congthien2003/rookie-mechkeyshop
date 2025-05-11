using Domain.Enum;
using Shared.ViewModels.Abstractions;

namespace Shared.ViewModels.Order
{
    public class OrderModel : BaseViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public IEnumerable<OrderItemModel> OrderItems { get; set; } = Enumerable.Empty<OrderItemModel>();
    }
}
