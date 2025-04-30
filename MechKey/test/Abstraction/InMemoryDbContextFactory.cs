using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Abstraction
{
    public class InMemoryDbContextFactory
    {
        public static InMemoryApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

            return new InMemoryApplicationDbContext(options);
        }
    }
}
