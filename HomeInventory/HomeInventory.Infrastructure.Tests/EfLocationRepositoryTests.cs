using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Repositories;
using Xunit;

namespace HomeInventory.Infrastructure.Tests.Repositories
{
    public class EfLocationRepositoryTests
    {
        private readonly Guid _ownerId = Guid.NewGuid();
        private readonly Guid _householdId = Guid.NewGuid();

        private EfLocationRepository CreateRepo()
            => new();

        private Location CreateLocation(string name, Guid? parentId = null, LocationType type = LocationType.Other)
        {
            return new Location(name, _householdId, _ownerId, parentId, type);
        }

        [Fact]
        public void Add_Then_GetById_ReturnsEntity()
        {
            var repo = CreateRepo();
            var loc = CreateLocation("Kitchen");

            repo.Add(loc);

            var loaded = repo.GetById(loc.Id);

            Assert.Equal(loc.Id, loaded.Id);
            Assert.Equal("Kitchen", loaded.Name);
        }

        [Fact]
        public void GetById_UnknownId_ThrowsKeyNotFoundException()
        {
            var repo = CreateRepo();

            Assert.Throws<KeyNotFoundException>(() => repo.GetById(Guid.NewGuid()));
        }

        [Fact]
        public void FindByName_WithoutParent_ReturnsAllMatches()
        {
            var repo = CreateRepo();

            var rootA = CreateLocation("RoomA");
            var rootB = CreateLocation("RoomB");
            repo.Add(rootA);
            repo.Add(rootB);

            var aBox = CreateLocation("Box", parentId: rootA.Id);
            var bBox = CreateLocation("Box", parentId: rootB.Id);
            repo.Add(aBox);
            repo.Add(bBox);

            var results = repo.FindByName("Box").ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, x => x.Id == aBox.Id);
            Assert.Contains(results, x => x.Id == bBox.Id);
        }

        [Fact]
        public void FindByName_WithParent_ReturnsOnlyMatchesUnderThatParent()
        {
            var repo = CreateRepo();

            var parent1 = CreateLocation("Parent1");
            var parent2 = CreateLocation("Parent2");
            repo.Add(parent1);
            repo.Add(parent2);

            var box1 = CreateLocation("Box", parentId: parent1.Id);
            var box2 = CreateLocation("Box", parentId: parent2.Id);
            repo.Add(box1);
            repo.Add(box2);

            var results = repo.FindByName("Box", parentId: parent1.Id).ToList();

            Assert.Single(results);
            Assert.Equal(box1.Id, results[0].Id);
        }

        [Fact]
        public void GetChildren_ReturnsOnlyDirectChildren()
        {
            var repo = CreateRepo();

            var parent = CreateLocation("Parent");
            repo.Add(parent);

            var child = CreateLocation("Child", parentId: parent.Id);
            repo.Add(child);

            var grandChild = CreateLocation("GrandChild", parentId: child.Id);
            repo.Add(grandChild);

            var children = repo.GetChildren(parent.Id).ToList();

            Assert.Single(children);
            Assert.Equal(child.Id, children[0].Id);
        }

        [Fact]
        public void GetChildren_Filter_ReturnsOnlyMatchingTypes()
        {
            var repo = CreateRepo();

            var parent = CreateLocation("Parent");
            repo.Add(parent);

            var drawer = CreateLocation("Drawer1", parentId: parent.Id, type: LocationType.Drawer);
            var shelf = CreateLocation("Shelf1", parentId: parent.Id, type: LocationType.Shelf);
            repo.Add(drawer);
            repo.Add(shelf);

            var filtered = repo.GetChildren(parent.Id, filter: new[] { LocationType.Drawer }).ToList();

            Assert.Single(filtered);
            Assert.Equal(drawer.Id, filtered[0].Id);
        }

        [Fact]
        public void GetRoots_ReturnsOnlyNullParentLocations()
        {
            var repo = CreateRepo();

            var root1 = CreateLocation("Root1");
            var root2 = CreateLocation("Root2");
            repo.Add(root1);
            repo.Add(root2);

            var child = CreateLocation("Child", parentId: root1.Id);
            repo.Add(child);

            var roots = repo.GetRoots().ToList();

            Assert.Equal(2, roots.Count);
            Assert.All(roots, r => Assert.Null(r.ParentLocationId));
        }

        [Fact]
        public void ExistsSiblingWithName_ReturnsTrue_WhenNameExistsUnderSameParent()
        {
            var repo = CreateRepo();

            var parent = CreateLocation("Parent");
            repo.Add(parent);

            repo.Add(CreateLocation("Box", parentId: parent.Id));

            Assert.True(repo.ExistsSiblingWithName(parent.Id, "Box"));
        }

        [Fact]
        public void ExistsSiblingWithName_ReturnsFalse_WhenNameExistsButUnderDifferentParent()
        {
            var repo = CreateRepo();

            var parent1 = CreateLocation("Parent1");
            var parent2 = CreateLocation("Parent2");
            repo.Add(parent1);
            repo.Add(parent2);

            repo.Add(CreateLocation("Box", parentId: parent1.Id));

            Assert.False(repo.ExistsSiblingWithName(parent2.Id, "Box"));
        }

        [Fact]
        public void ExistsSiblingWithName_ExcludeId_IgnoresThatLocation()
        {
            var repo = CreateRepo();

            var parent = CreateLocation("Parent");
            repo.Add(parent);

            var loc = CreateLocation("Box", parentId: parent.Id);
            repo.Add(loc);

            // When renaming the same location to its current name, excludeId should prevent false positive.
            Assert.False(repo.ExistsSiblingWithName(parent.Id, "Box", excludeLocationId: loc.Id));
        }

        [Fact]
        public void GetAncestors_ReturnsParentThenRoot()
        {
            var repo = CreateRepo();

            var root = CreateLocation("Root");
            repo.Add(root);

            var child = CreateLocation("Child", parentId: root.Id);
            repo.Add(child);

            var leaf = CreateLocation("Leaf", parentId: child.Id);
            repo.Add(leaf);

            var ancestors = repo.GetAncestors(leaf.Id).ToList();

            Assert.Equal(2, ancestors.Count);
            Assert.Equal(child.Id, ancestors[0].Id);
            Assert.Equal(root.Id, ancestors[1].Id);
        }

        [Fact]
        public void IsDescendantOf_ReturnsTrue_ForDescendant()
        {
            var repo = CreateRepo();

            var root = CreateLocation("Root");
            repo.Add(root);

            var child = CreateLocation("Child", parentId: root.Id);
            repo.Add(child);

            var leaf = CreateLocation("Leaf", parentId: child.Id);
            repo.Add(leaf);

            Assert.True(repo.IsDescendantOf(leaf.Id, root.Id));
            Assert.True(repo.IsDescendantOf(leaf.Id, child.Id));
            Assert.False(repo.IsDescendantOf(root.Id, leaf.Id));
        }

        [Fact]
        public void GetSubtree_IncludeRootTrue_ReturnsRootAndAllDescendants()
        {
            var repo = CreateRepo();

            var root = CreateLocation("Root");
            repo.Add(root);

            var child1 = CreateLocation("Child1", parentId: root.Id);
            var child2 = CreateLocation("Child2", parentId: root.Id);
            var leaf = CreateLocation("Leaf", parentId: child1.Id);
            repo.Add(child1);
            repo.Add(child2);
            repo.Add(leaf);

            var nodes = repo.GetSubtree(root.Id, includeRoot: true).ToList();

            Assert.Contains(nodes, x => x.Id == root.Id);
            Assert.Contains(nodes, x => x.Id == child1.Id);
            Assert.Contains(nodes, x => x.Id == child2.Id);
            Assert.Contains(nodes, x => x.Id == leaf.Id);
        }

        [Fact]
        public void GetSubtree_IncludeRootFalse_DoesNotReturnRoot()
        {
            var repo = CreateRepo();

            var root = CreateLocation("Root");
            repo.Add(root);

            var child = CreateLocation("Child", parentId: root.Id);
            repo.Add(child);

            var nodes = repo.GetSubtree(root.Id, includeRoot: false).ToList();

            Assert.DoesNotContain(nodes, x => x.Id == root.Id);
            Assert.Contains(nodes, x => x.Id == child.Id);
        }

        [Fact]
        public void Search_IsCaseInsensitiveContains_AndRespectsLimit()
        {
            var repo = CreateRepo();

            repo.Add(CreateLocation("Kitchen"));
            repo.Add(CreateLocation("kitchen drawer"));
            repo.Add(CreateLocation("Garage"));

            var results = repo.Search("KITC", limit: 1).ToList();

            Assert.Single(results);
            Assert.True(results[0].Name.Contains("KITC", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Search_WithinParentId_FindsOnlyInThatScope()
        {
            var repo = CreateRepo();

            var kitchen = CreateLocation("Kitchen");
            var garage = CreateLocation("Garage");
            repo.Add(kitchen);
            repo.Add(garage);

            var kBox = CreateLocation("Box", parentId: kitchen.Id);
            var gBox = CreateLocation("Box", parentId: garage.Id);
            repo.Add(kBox);
            repo.Add(gBox);

            var results = repo.Search("Box", withinParentId: kitchen.Id).ToList();

            Assert.Contains(results, x => x.Id == kBox.Id);
            Assert.DoesNotContain(results, x => x.Id == gBox.Id);
        }

        [Fact]
        public void Update_PersistsChanges()
        {
            var repo = CreateRepo();

            var loc = CreateLocation("Old");
            repo.Add(loc);

            loc.Rename("New");
            repo.Update(loc);

            var loaded = repo.GetById(loc.Id);
            Assert.Equal("New", loaded.Name);
        }

        [Fact]
        public void Remove_Then_GetById_Throws()
        {
            var repo = CreateRepo();

            var loc = CreateLocation("ToRemove");
            repo.Add(loc);

            repo.Remove(loc);

            Assert.Throws<KeyNotFoundException>(() => repo.GetById(loc.Id));
        }

        [Fact]
        public void Remove_RemovesSubtree_IfRootRemoved()
        {
            // This is a design choice. For tree repos it's often expected.
            // If you DON'T want cascade delete, delete this test.
            var repo = CreateRepo();

            var root = CreateLocation("Root");
            repo.Add(root);

            var child = CreateLocation("Child", parentId: root.Id);
            var leaf = CreateLocation("Leaf", parentId: child.Id);
            repo.Add(child);
            repo.Add(leaf);

            repo.Remove(root);

            Assert.Throws<KeyNotFoundException>(() => repo.GetById(root.Id));
            Assert.Throws<KeyNotFoundException>(() => repo.GetById(child.Id));
            Assert.Throws<KeyNotFoundException>(() => repo.GetById(leaf.Id));
        }
    }
}
