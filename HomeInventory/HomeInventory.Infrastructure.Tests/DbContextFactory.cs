using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Tests
{
    public static class DbContextFactory
    {
        public static HomeInventoryDbContext Create(string connectionString)
        {
            var options = new DbContextOptionsBuilder<HomeInventoryDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            var ctx = new HomeInventoryDbContext(options);
            ctx.Database.Migrate();
            return ctx;
        }
    }
}
