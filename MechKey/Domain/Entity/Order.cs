
using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public double TotalAmount { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}