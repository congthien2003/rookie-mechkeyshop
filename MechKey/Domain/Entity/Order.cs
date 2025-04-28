
using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public OrderStatus Status { get; set; }
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

        public void ChangeStatus(int status)
        {
            switch (status)
            {
                case 0:
                    Status = OrderStatus.Pending;
                    break;
                case 1:
                    Status = OrderStatus.Accepted;
                    break;
                case 2:
                    Status = OrderStatus.Cancelled;
                    break;
                case 3:
                    Status = OrderStatus.Completed;
                    break;
                default: return;
            }
        }
    }
}