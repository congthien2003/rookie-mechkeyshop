using System.Diagnostics.CodeAnalysis;

namespace Domain.Entity
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public double TotalAmount { get; set; }
        [AllowNull]
        public string Option { get; set; } = string.Empty;
    }
}