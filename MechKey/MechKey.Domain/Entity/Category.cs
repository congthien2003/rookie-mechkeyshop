using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}