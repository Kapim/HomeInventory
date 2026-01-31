using HomeInventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence
{
    public class HomeInventoryDbContext(DbContextOptions<HomeInventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Location> Locations => Set<Location>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeInventoryDbContext).Assembly);
        }
    }
}
