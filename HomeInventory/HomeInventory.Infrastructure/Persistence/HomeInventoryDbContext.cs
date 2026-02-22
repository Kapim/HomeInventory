using HomeInventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence
{
    public class HomeInventoryDbContext(DbContextOptions<HomeInventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Household> Households => Set<Household>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeInventoryDbContext).Assembly);
        }
    }
}
