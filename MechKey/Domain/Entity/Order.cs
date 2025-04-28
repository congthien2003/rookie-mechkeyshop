
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
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Order()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public void ChangeStatus(string status)
        {
            switch (status)
            {
                case "Pending":
                    Status = OrderStatus.Pending;
                    break;
                case "Accepted":
                    Status = OrderStatus.Accepted;
                    break;
                case "Canceled":
                    Status = OrderStatus.Cancelled;
                    break;
                case "Completed":
                    Status = OrderStatus.Completed;
                    break;
                default: return;
            }
        }
    }
}