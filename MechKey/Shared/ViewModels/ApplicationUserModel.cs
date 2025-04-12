namespace Shared.ViewModels
{
    public class ApplicationUserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phones { get; set; }
        public string Address { get; set; }
        public int UserRoleId { get; set; }

    }
}
