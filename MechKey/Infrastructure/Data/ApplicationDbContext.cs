using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace MechkeyShop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
