using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Repositories;
using Xunit;

namespace HomeInventory.Infrastructure.Tests
{
    [Collection(nameof(PostgresCollection))]
    public class EfItemRepositoryTests(PostgresFixture fx)
    {
        private readonly PostgresFixture _fx = fx;
        private readonly Guid _ownerId = Guid.NewGuid();
        private readonly Guid _otherOwnerId = Guid.NewGuid();
        private readonly Guid _householdId = Guid.NewGuid();
        private readonly Guid _locationId = Guid.NewGuid();

        private (HomeInventoryDbContext ctx, EfItemRepository repo) CreateSut()
        {
            var ctx = DbContextFactory.Create(_fx.ConnectionString);

            // Clean DB between tests (simple approach)
            ctx.Items.RemoveRange(ctx.Items);
            ctx.SaveChanges();

            var repo = new EfItemRepository(ctx);
            return (ctx, repo);
        }

        private Item CreateItem(string name, Guid? locationId = null, Guid? ownerId = null, int quantity = 1)
        {
            var item = new Item(name, ownerId ?? _ownerId, locationId ?? _locationId)
            {
                Quantity = quantity
            };


            return item;
        }

        private Location CreateLocation(string name, Guid? parentId = null, LocationType type = LocationType.Other)
        {
            return new Location(name, _householdId, _ownerId, parentId, type);
        }

        [Fact]
        public async Task AddAsync_persists_item_and_GetByIdAsync_returns_it()
        {
            var (ctx, repo) = CreateSut();

            var item = CreateItem("Batteries", quantity: 5);

            await repo.AddAsync(item, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var loaded = await repo.GetByIdAsync(item.Id, CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal(item.Id, loaded!.Id);
            Assert.Equal("Batteries", loaded.Name);
            Assert.Equal(_ownerId, loaded.OwnerUserId);
            Assert.Equal(5, loaded.Quantity);
        }

        [Fact]
        public async Task GetByIdAsync_returns_null_when_not_found()
        {
            var (_, repo) = CreateSut();

            var loaded = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.Null(loaded);
        }

        [Fact]
        public async Task ExistsAsync_returns_true_for_existing_item_and_false_otherwise()
        {
            var (ctx, repo) = CreateSut();

            var item = CreateItem("Hammer");
            await repo.AddAsync(item, CancellationToken.None);
            await ctx.SaveChangesAsync();

            var exists = await repo.ExistsAsync(item.Id, CancellationToken.None);
            var missing = await repo.ExistsAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.True(exists);
            Assert.False(missing);
        }

        [Fact]
        public async Task GetAllForUserAsync_returns_only_items_for_given_owner()
        {
            var (ctx, repo) = CreateSut();

            var a = CreateItem("AA Batteries", ownerId: _ownerId);
            var b = CreateItem("AAA Batteries", ownerId: _ownerId);
            var c = CreateItem("Other owner item", ownerId: _otherOwnerId);

            await repo.AddAsync(a, CancellationToken.None);
            await repo.AddAsync(b, CancellationToken.None);
            await repo.AddAsync(c, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var results = await repo.GetAllForUserAsync(_ownerId, CancellationToken.None);

            Assert.Equal(2, results.Count);
            Assert.All(results, i => Assert.Equal(_ownerId, i.OwnerUserId));
            Assert.Contains(results, i => i.Name == "AA Batteries");
            Assert.Contains(results, i => i.Name == "AAA Batteries");
            Assert.DoesNotContain(results, i => i.Name == "Other owner item");
        }

        [Fact]
        public async Task FindByNameAsync_returns_matches_for_owner()
        {
            var (ctx, repo) = CreateSut();

            var a = CreateItem("AA Batteries", ownerId: _ownerId);
            var b = CreateItem("AAA Batteries", ownerId: _ownerId);
            var c = CreateItem("Screws", ownerId: _ownerId);
            var d = CreateItem("AA Batteries", ownerId: _otherOwnerId);

            await repo.AddAsync(a, CancellationToken.None);
            await repo.AddAsync(b, CancellationToken.None);
            await repo.AddAsync(c, CancellationToken.None);
            await repo.AddAsync(d, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var results = await repo.FindByNameAsync(_ownerId, "Batteries", CancellationToken.None);

            Assert.NotNull(results);
            Assert.All(results, i => Assert.Equal(_ownerId, i.OwnerUserId));
            Assert.Contains(results, i => i.Name == "AA Batteries");
            Assert.Contains(results, i => i.Name == "AAA Batteries");
            Assert.DoesNotContain(results, i => i.Name == "AA Batteries" && i.OwnerUserId == _otherOwnerId);
        }

        [Fact]
        public async Task GetByLocationAsync_returns_only_items_in_location_for_owner()
        {
            var (ctx, repo) = CreateSut();

            var location = CreateLocation("Garage");
            ctx.Locations.Add(location);
            await ctx.SaveChangesAsync();

            var inGarage1 = CreateItem("Hammer", locationId: location.Id, ownerId: _ownerId);
            var inGarage2 = CreateItem("Screwdriver", locationId: location.Id, ownerId: _ownerId);
            var elsewhere = CreateItem("Tape", locationId: null, ownerId: _ownerId);
            var otherOwnerInGarage = CreateItem("OtherOwnerItem", locationId: location.Id, ownerId: _otherOwnerId);

            await repo.AddAsync(inGarage1, CancellationToken.None);
            await repo.AddAsync(inGarage2, CancellationToken.None);
            await repo.AddAsync(elsewhere, CancellationToken.None);
            await repo.AddAsync(otherOwnerInGarage, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var results = await repo.GetByLocationAsync(_ownerId, location.Id, CancellationToken.None);

            Assert.Equal(2, results.Count);
            Assert.All(results, i =>
            {
                Assert.Equal(_ownerId, i.OwnerUserId);
                Assert.Equal(location.Id, i.LocationId);
            });

            Assert.Contains(results, i => i.Name == "Hammer");
            Assert.Contains(results, i => i.Name == "Screwdriver");
            Assert.DoesNotContain(results, i => i.Name == "OtherOwnerItem");
            Assert.DoesNotContain(results, i => i.Name == "Tape");
        }

        [Fact]
        public async Task UpdateAsync_persists_rename_and_quantity_change()
        {
            var (ctx, repo) = CreateSut();

            var item = CreateItem("Old name", quantity: 1);
            await repo.AddAsync(item, CancellationToken.None);
            await ctx.SaveChangesAsync();

            item.Rename("New name");
            item.Quantity = 42;

            await repo.UpdateAsync(item, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var loaded = await repo.GetByIdAsync(item.Id, CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal("New name", loaded!.Name);
            Assert.Equal(42, loaded.Quantity);
        }

        [Fact]
        public async Task UpdateAsync_persists_MoveToLocation()
        {
            var (ctx, repo) = CreateSut();

            var location = CreateLocation("Kitchen");
            ctx.Locations.Add(location);
            await ctx.SaveChangesAsync();

            var item = CreateItem("Batteries", locationId: null);
            await repo.AddAsync(item, CancellationToken.None);
            await ctx.SaveChangesAsync();

            item.MoveToLocation(location.Id);

            await repo.UpdateAsync(item, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var loaded = await repo.GetByIdAsync(item.Id, CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal(location.Id, loaded!.LocationId);
        }

        [Fact]
        public async Task DeleteAsync_removes_item()
        {
            var (ctx, repo) = CreateSut();

            var item = CreateItem("To be deleted");
            await repo.AddAsync(item, CancellationToken.None);
            await ctx.SaveChangesAsync();

            await repo.DeleteAsync(item.Id, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var loaded = await repo.GetByIdAsync(item.Id, CancellationToken.None);
            Assert.Null(loaded);
        }
    }
}