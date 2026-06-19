using Domain.Entity;
using MechKey.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Category> Categories { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductRating> ProductRatings { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<ProductImage> ProductImages { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
