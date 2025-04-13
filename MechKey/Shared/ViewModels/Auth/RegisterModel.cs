namespace Shared.ViewModels.Auth
{
    public class RegisterModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phones { get; set; }
        public string Address { get; set; }
        public int RoleId { get; set; } = 2;
    }
}
