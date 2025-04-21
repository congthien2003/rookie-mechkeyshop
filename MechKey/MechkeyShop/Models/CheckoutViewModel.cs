namespace MechkeyShop.Models
{
    public class CheckoutViewModel
    {
        public UserModel User { get; set; }
        public List<CartItemModel> CartItems { get; set; }
    }
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class CartItemModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
