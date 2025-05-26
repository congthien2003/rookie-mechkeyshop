
namespace Shared.ViewModels.Auth
{
    public class ApplicationUserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phones { get; set; }
        public string Address { get; set; }
        public int RoleId { get; set; }
        public bool IsEmailConfirmed { get; set; }

    }
}
