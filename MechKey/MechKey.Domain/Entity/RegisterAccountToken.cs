using Domain.Entity;

namespace MechKey.Domain.Entity
{
    public class RegisterAccountToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Token { get; set; }
        public bool IsAccess { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddDays(3);

    }
}
