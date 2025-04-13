using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class ApplicationUser : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255, ErrorMessage = "Email must be less than 255 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [StringLength(512, ErrorMessage = "Password must be less than 100 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Salting is required")]
        public string Salting { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string Phones { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int RoleId { get; set; }
        public UserRole UserRoles { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}