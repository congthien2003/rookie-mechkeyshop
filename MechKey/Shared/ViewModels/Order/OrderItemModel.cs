namespace Shared.ViewModels.Order
{
    public class OrderItemModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public OrderItemVariant Option { get; set; }
    }

    public class OrderItemVariant
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}