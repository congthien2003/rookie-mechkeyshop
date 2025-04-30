using Domain.Common;
using Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public Category(Guid id, string name)
        {
            Id = id;
            Name = name ?? throw new CategoryInvalidDataException();
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}