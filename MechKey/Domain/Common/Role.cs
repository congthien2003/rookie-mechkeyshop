using System.ComponentModel.DataAnnotations;

namespace Domain.Common
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}