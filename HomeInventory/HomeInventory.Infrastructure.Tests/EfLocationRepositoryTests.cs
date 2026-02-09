using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Repositories;
using Xunit;

namespace HomeInventory.Infrastructure.Tests
{
    [Collection(nameof(PostgresCollection))]
    public class EfLocationRepositoryTests(PostgresFixture fx)
    {
        private readonly PostgresFixture _fx = fx;
        private readonly Guid _ownerId = Guid.NewGuid();
        private readonly Guid _householdId = Guid.NewGuid();

        private (HomeInventoryDbContext ctx, EfLocationRepository repo) CreateSut()
        {
            var ctx = DbContextFactory.Create(_fx.ConnectionString);

            // Clean DB between tests (simple approach)
            ctx.Locations.RemoveRange(ctx.Locations);
            ctx.SaveChanges();

            var repo = new EfLocationRepository(ctx);
            return (ctx, repo);
        }

        private Location CreateLocation(string name, Guid? parentId = null, LocationType type = LocationType.Other)
        {
            return new Location(name, _householdId, _ownerId, parentId, type);
        }

        [Fact]
        public async Task Add_Then_GetById_ReturnsEntity()
        {
            var (_, repo) = CreateSut();
            var loc = CreateLocation("Kitchen");

            await repo.AddAsync(loc);

            var loaded = await repo.GetByIdAsync(loc.Id);

            Assert.Equal(loc.Id, loaded.Id);
            Assert.Equal("Kitchen", loaded.Name);
        }

        [Fact]
        public async Task GetById_UnknownId_ThrowsKeyNotFoundException()
        {
            var (_, repo) = CreateSut();

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task FindByName_WithoutParent_ReturnsAllMatches()
        {
            var (_, repo) = CreateSut();

            var rootA = CreateLocation("RoomA");
            var rootB = CreateLocation("RoomB");
            await repo.AddAsync(rootA);
            await repo.AddAsync(rootB);

            var aBox = CreateLocation("Box", parentId: rootA.Id);
            var bBox = CreateLocation("Box", parentId: rootB.Id);
            await repo.AddAsync(aBox);
            await repo.AddAsync(bBox);

            var results = (await repo.FindByNameAsync("Box")).ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, x => x.Id == aBox.Id);
            Assert.Contains(results, x => x.Id == bBox.Id);
        }

        [Fact]
        public async Task FindByName_WithParent_ReturnsOnlyMatchesUnderThatParent()
        {
            var (_, repo) = CreateSut();

            var parent1 = CreateLocation("Parent1");
            var parent2 = CreateLocation("Parent2");
            await repo.AddAsync(parent1);
            await repo.AddAsync(parent2);

            var box1 = CreateLocation("Box", parentId: parent1.Id);
            var box2 = CreateLocation("Box", parentId: parent2.Id);
            await repo.AddAsync(box1);
            await repo.AddAsync(box2);

            var results = (await repo.FindByNameAsync("Box", parentId: parent1.Id)).ToList();

            Assert.Single(results);
            Assert.Equal(box1.Id, results[0].Id);
        }

        [Fact]
        public async Task GetChildren_ReturnsOnlyDirectChildren()
        {
            var (_, repo) = CreateSut();

            var parent = CreateLocation("Parent");
            await repo.AddAsync(parent);

            var child = CreateLocation("Child", parentId: parent.Id);
            await repo.AddAsync(child);

            var grandChild = CreateLocation("GrandChild", parentId: child.Id);
            await repo.AddAsync(grandChild);

            var children = (await repo.GetChildrenAsync(parent.Id)).ToList();

            Assert.Single(children);
            Assert.Equal(child.Id, children[0].Id);
        }

        [Fact]
        public async Task GetChildren_Filter_ReturnsOnlyMatchingTypes()
        {
            var (_, repo) = CreateSut();

            var parent = CreateLocation("Parent");
            await repo.AddAsync(parent);

            var drawer = CreateLocation("Drawer1", parentId: parent.Id, type: LocationType.Drawer);
            var shelf = CreateLocation("Shelf1", parentId: parent.Id, type: LocationType.Shelf);
            await repo.AddAsync(drawer);
            await repo.AddAsync(shelf);

            var filtered = (await repo.GetChildrenAsync(parent.Id, filter: new[] { LocationType.Drawer })).ToList();

            Assert.Single(filtered);
            Assert.Equal(drawer.Id, filtered[0].Id);
        }

        [Fact]
        public async Task GetRoots_ReturnsOnlyNullParentLocations()
        {
            var (_, repo) = CreateSut();

            var root1 = CreateLocation("Root1");
            var root2 = CreateLocation("Root2");
            await repo.AddAsync(root1);
            await repo.AddAsync(root2);

            var child = CreateLocation("Child", parentId: root1.Id);
            await repo.AddAsync(child);

            var roots = (await repo.GetRootsAsync()).ToList();

            Assert.Equal(2, roots.Count);
            Assert.All(roots, r => Assert.Null(r.ParentLocationId));
        }

        [Fact]
        public async Task ExistsSiblingWithName_ReturnsTrue_WhenNameExistsUnderSameParent()
        {
            var (_, repo) = CreateSut();

            var parent = CreateLocation("Parent");
            await repo.AddAsync(parent);

            await repo.AddAsync(CreateLocation("Box", parentId: parent.Id));

            Assert.True(await repo.ExistsSiblingWithNameAsync(parent.Id, "Box"));
        }

        [Fact]
        public async Task ExistsSiblingWithName_ReturnsFalse_WhenNameExistsButUnderDifferentParent()
        {
            var (_, repo) = CreateSut();

            var parent1 = CreateLocation("Parent1");
            var parent2 = CreateLocation("Parent2");
            await repo.AddAsync(parent1);
            await repo.AddAsync(parent2);

            await repo.AddAsync(CreateLocation("Box", parentId: parent1.Id));

            Assert.False(await repo.ExistsSiblingWithNameAsync(parent2.Id, "Box"));
        }

        [Fact]
        public async Task ExistsSiblingWithName_ExcludeId_IgnoresThatLocation()
        {
            var (_, repo) = CreateSut();

            var parent = CreateLocation("Parent");
            await repo.AddAsync(parent);

            var loc = CreateLocation("Box", parentId: parent.Id);
            await repo.AddAsync(loc);

            // When renaming the same location to its current name, excludeId should prevent false positive.
            Assert.False(await repo.ExistsSiblingWithNameAsync(parent.Id, "Box", excludeLocationId: loc.Id));
        }

        [Fact]
        public async Task GetAncestors_ReturnsParentThenRoot()
        {
            var (_, repo) = CreateSut();

            var root = CreateLocation("Root");
            await repo.AddAsync(root);

            var child = CreateLocation("Child", parentId: root.Id);
            await repo.AddAsync(child);

            var leaf = CreateLocation("Leaf", parentId: child.Id);
            await repo.AddAsync(leaf);

            var ancestors = (await repo.GetAncestorsAsync(leaf.Id)).ToList();

            Assert.Equal(2, ancestors.Count);
            Assert.Equal(child.Id, ancestors[0].Id);
            Assert.Equal(root.Id, ancestors[1].Id);
        }

        [Fact]
        public async Task IsDescendantOf_ReturnsTrue_ForDescendant()
        {
            var (_, repo) = CreateSut();

            var root = CreateLocation("Root");
            await repo.AddAsync(root);

            var child = CreateLocation("Child", parentId: root.Id);
            await repo.AddAsync(child);

            var leaf = CreateLocation("Leaf", parentId: child.Id);
            await repo.AddAsync(leaf);

            Assert.True(await repo.IsDescendantOfAsync(leaf.Id, root.Id));
            Assert.True(await repo.IsDescendantOfAsync(leaf.Id, child.Id));
            Assert.False(await repo.IsDescendantOfAsync(root.Id, leaf.Id));
        }

        [Fact]
        public async Task GetSubtree_IncludeRootTrue_ReturnsRootAndAllDescendants()
        {
            var (_, repo) = CreateSut();

            var root = CreateLocation("Root");
            await repo.AddAsync(root);

            var child1 = CreateLocation("Child1", parentId: root.Id);
            var child2 = CreateLocation("Child2", parentId: root.Id);
            var leaf = CreateLocation("Leaf", parentId: child1.Id);
            await repo.AddAsync(child1);
            await repo.AddAsync(child2);
            await repo.AddAsync(leaf);

            var nodes = (await repo.GetSubtreeAsync(root.Id, includeRoot: true)).ToList();

            Assert.Contains(nodes, x => x.Id == root.Id);
            Assert.Contains(nodes, x => x.Id == child1.Id);
            Assert.Contains(nodes, x => x.Id == child2.Id);
            Assert.Contains(nodes, x => x.Id == leaf.Id);
        }

        [Fact]
        public async Task GetSubtree_IncludeRootFalse_DoesNotReturnRoot()
        {
            var (_, repo) = CreateSut();

            var root = CreateLocation("Root");
            await repo.AddAsync(root);

            var child = CreateLocation("Child", parentId: root.Id);
            await repo.AddAsync(child);

            var nodes = (await repo.GetSubtreeAsync(root.Id, includeRoot: false)).ToList();

            Assert.DoesNotContain(nodes, x => x.Id == root.Id);
            Assert.Contains(nodes, x => x.Id == child.Id);
        }

        [Fact]
        public async Task Search_IsCaseInsensitiveContains_AndRespectsLimit()
        {
            var (_, repo) = CreateSut();

            await repo.AddAsync(CreateLocation("Kitchen"));
            await repo.AddAsync(CreateLocation("kitchen drawer"));
            await repo.AddAsync(CreateLocation("Garage"));

            var results = (await repo.SearchAsync("KITC", limit: 1)).ToList();

            Assert.Single(results);
            Assert.True(results[0].Name.Contains("KITC", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Search_WithinParentId_FindsOnlyInThatScope()
        {
            var (_, repo) = CreateSut();

            var kitchen = CreateLocation("Kitchen");
            var garage = CreateLocation("Garage");
            await repo.AddAsync(kitchen);
            await repo.AddAsync(garage);

            var kBox = CreateLocation("Box", parentId: kitchen.Id);
            var gBox = CreateLocation("Box", parentId: garage.Id);
            await repo.AddAsync(kBox);
            await repo.AddAsync(gBox);

            var results = (await repo.SearchAsync("Box", withinParentId: kitchen.Id)).ToList();

            Assert.Contains(results, x => x.Id == kBox.Id);
            Assert.DoesNotContain(results, x => x.Id == gBox.Id);
        }

        [Fact]
        public async Task Update_PersistsChanges()
        {
            var (_, repo) = CreateSut();

            var loc = CreateLocation("Old");
            await repo.AddAsync(loc);

            loc.Rename("New");
            await repo.UpdateAsync(loc);

            var loaded = await repo.GetByIdAsync(loc.Id);
            Assert.Equal("New", loaded.Name);
        }

        [Fact]
        public async Task Remove_Then_GetById_Throws()
        {
            var (_, repo) = CreateSut();

            var loc = CreateLocation("ToRemove");
            await repo.AddAsync(loc);

            await repo.RemoveAsync(loc);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetByIdAsync(loc.Id));
        }

        [Fact]
        public async Task Remove_RemovesSubtree_IfRootRemoved()
        {
            
            var (_, repo) = CreateSut();

            var root = CreateLocation("Root");
            await repo.AddAsync(root);

            var child = CreateLocation("Child", parentId: root.Id);
            var leaf = CreateLocation("Leaf", parentId: child.Id);
            await repo.AddAsync(child);
            await repo.AddAsync(leaf);

            await repo.RemoveAsync(root);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetByIdAsync(root.Id));
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetByIdAsync(child.Id));
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetByIdAsync(leaf.Id));
        }
    }
}
