namespace Shared.ViewModels.Product
{
    public class ProductRatingModel
    {
        public int Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; } = "";
        public DateTime RatedAt { get; set; } = DateTime.UtcNow;
        // Foreign key
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
