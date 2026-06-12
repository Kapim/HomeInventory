using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.ViewModels;
using HomeInventory.Wpf.Tests;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class LocationTreeViewModelTests
    {
        private readonly Guid _householdId = Guid.NewGuid();

        private (LocationTreeViewModel vm, MockLocationsService locations, MockHouseholdsService households, MockDialogService dialogs)
            CreateSut(IEnumerable<LocationListItem>? locationItems = null)
        {
            var locations = new MockLocationsService();
            var households = new MockHouseholdsService();
            var dialogs = new MockDialogService();
            households.SetLocations(locationItems ?? []);
            var vm = new LocationTreeViewModel(households, locations, new ErrorLocalizerService(), dialogs);
            return (vm, locations, households, dialogs);
        }

        private LocationListItem Item(Guid id, string name, Guid? parentId = null, int sortOrder = 0)
            => new(id, name, LocationType.Other, parentId, sortOrder);

        [Fact]
        public async Task MoveLocation_SetsIsMovingLocationFlag()
        {
            var rootId = Guid.NewGuid();
            var (vm, _, _, _) = CreateSut([Item(rootId, "Root")]);
            await vm.LoadAsync(_householdId, CancellationToken.None);

            vm.MoveLocationCommand.Execute(null);

            Assert.True(vm.IsMovingLocation);
        }

        [Fact]
        public async Task CancelMoveLocation_ClearsFlag()
        {
            var rootId = Guid.NewGuid();
            var (vm, _, _, _) = CreateSut([Item(rootId, "Root")]);
            await vm.LoadAsync(_householdId, CancellationToken.None);

            vm.MoveLocationCommand.Execute(null);
            vm.CancelMoveLocationCommand.Execute(null);

            Assert.False(vm.IsMovingLocation);
        }

        [Fact]
        public async Task MoveLocation_SelectingTarget_CallsMoveAsync()
        {
            var parentId = Guid.NewGuid();
            var childId = Guid.NewGuid();
            var (vm, locations, _, _) = CreateSut([
                Item(parentId, "Parent"),
                Item(childId, "Child")
            ]);
            await vm.LoadAsync(_householdId, CancellationToken.None);

            vm.SelectedLocation = vm.RootLocations.First(x => x.Id == childId);
            vm.MoveLocationCommand.Execute(null);

            vm.SelectedLocation = vm.RootLocations.First(x => x.Id == parentId);

            await Task.Delay(100);

            Assert.Single(locations.MoveCalls);
            Assert.Equal(childId, locations.MoveCalls[0].Id);
            Assert.Equal(parentId, locations.MoveCalls[0].NewParentId);
        }

        [Fact]
        public async Task MoveLocationToRoot_CallsMoveAsyncWithNullParent()
        {
            var parentId = Guid.NewGuid();
            var childId = Guid.NewGuid();
            var (vm, locations, _, _) = CreateSut([
                Item(parentId, "Parent"),
                Item(childId, "Child", parentId)
            ]);
            await vm.LoadAsync(_householdId, CancellationToken.None);

            vm.SelectedLocation = vm.RootLocations.First(x => x.Id == parentId)
                .Children.First(x => x.Id == childId);
            vm.MoveLocationCommand.Execute(null);
            await vm.MoveLocationToRootCommand.ExecuteAsync(null);

            Assert.Single(locations.MoveCalls);
            Assert.Equal(childId, locations.MoveCalls[0].Id);
            Assert.Null(locations.MoveCalls[0].NewParentId);
        }

        [Fact]
        public async Task MoveLocation_ToItself_DoesNotCallMoveAsync()
        {
            var rootId = Guid.NewGuid();
            var (vm, locations, _, _) = CreateSut([Item(rootId, "Root")]);
            await vm.LoadAsync(_householdId, CancellationToken.None);

            vm.MoveLocationCommand.Execute(null);
            vm.SelectedLocation = vm.RootLocations.First(x => x.Id == rootId);

            await Task.Delay(100);

            Assert.Empty(locations.MoveCalls);
        }

        [Fact]
        public async Task MoveLocation_ToDescendant_ShowsError()
        {
            var parentId = Guid.NewGuid();
            var childId = Guid.NewGuid();
            var (vm, locations, _, dialogs) = CreateSut([
                Item(parentId, "Parent"),
                Item(childId, "Child", parentId)
            ]);
            await vm.LoadAsync(_householdId, CancellationToken.None);

            var parentNode = vm.RootLocations.First(x => x.Id == parentId);
            vm.SelectedLocation = parentNode;
            vm.MoveLocationCommand.Execute(null);

            vm.SelectedLocation = parentNode.Children.First(x => x.Id == childId);

            await Task.Delay(100);

            Assert.Empty(locations.MoveCalls);
            Assert.NotEmpty(dialogs.Messages);
        }
    }
}
