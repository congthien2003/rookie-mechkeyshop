using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Not empty")]
        public string Name { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}