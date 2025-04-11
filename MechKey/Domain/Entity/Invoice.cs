
using Domain.Common;
using Domain.Entity;

namespace Application.Entity
{
    public class Invoice : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public bool IsPaid { get; set; } = false;
        public double TotalAmount { get; set; }
        public DateTime? PaidAt { get; set; } = null;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}