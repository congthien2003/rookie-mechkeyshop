using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}