using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Abstraction
{
    public class InMemoryApplicationDbContext : ApplicationDbContext
    {
        public InMemoryApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    }
}
