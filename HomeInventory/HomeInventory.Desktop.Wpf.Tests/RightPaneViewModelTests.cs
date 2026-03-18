using HomeInventory.Client.Models;
using HomeInventory.Client.Services;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Enums;
using HomeInventory.Desktop.Wpf.ViewModels;
using HomeInventory.Wpf.Tests;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class RightPaneViewModelTests
    {
        private static async Task WaitUntilAsync(Func<bool> condition, int timeoutMs = 1500, int pollMs = 20)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (condition()) return;
                await Task.Delay(pollMs);
            }
            throw new TimeoutException("Expected state was not reached in time.");
        }

        [Fact]
        public async Task CreateNewItem_AddsNewItemAndBindsId()
        {
            var vm = CreateRightPaneVM(out var _, out var _);
            await vm.LoadAsync(new(Guid.NewGuid(), "location", null, 0));
            vm.AddNewItem();
            Assert.NotEmpty(vm.Items);
            var item = vm.Items[0];
            Assert.True(item.IsNew);
            Assert.Null(item.Item);
            item.Name = "NewName";
            await WaitUntilAsync(() => item.Item is not null && item.Item.Name == "NewName");
            Assert.NotNull(item.Item);
            Assert.False(item.IsNew);
            Assert.False(vm.IsBusy);
        }

        [Fact]
        public async Task CreateNewItem_ThenUpdateWorks()
        {
            var vm = CreateRightPaneVM(out var _, out var _);
            await vm.LoadAsync(new(Guid.NewGuid(), "location", null, 0));
            vm.AddNewItem();
            var item = vm.Items[0];
            item.Name = "NewName";
            await WaitUntilAsync(() => item.Item is not null && item.Item.Name == "NewName");
            item.Name = "NewName2";
            await WaitUntilAsync(() => item.Item is not null && item.Item.Name == "NewName2");
            Assert.Equal("NewName2", item.Item!.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        public async Task CreateNewItem_InvalidName_ShowsErrorDataRemains(string? name)
        {
            var vm = CreateRightPaneVM(out var _, out var mockDialogService);
            await vm.LoadAsync(new(Guid.NewGuid(), "location", null, 0));
            vm.AddNewItem();
            var item = vm.Items[0];
            item.Description = "desc";
            item.PlacementNote = "place";
            item.Quantity = 10;
            item.Name = name;
            await WaitUntilAsync(() => mockDialogService.Messages.Count != 0);
            Assert.True(item.IsNew);
            Assert.Null(item.Item);
            Assert.Equal("desc", item.Description);
            Assert.Equal("place", item.PlacementNote);
            Assert.Equal(10, item.Quantity);
            Assert.Single(mockDialogService.Messages);
        }

        [Fact]
        public async Task UpdateItem_ItemChanged()
        {
            var guids = new List<Guid> {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()};
            var locationId = Guid.NewGuid();
            var vm = CreateRightPaneVM(out var _, out var _, guids, locationId);
            await vm.LoadAsync(new(locationId, "location", null, 0));
            var item = vm.Items.FirstOrDefault(x => x.Item?.Id == guids[0]);
            Assert.NotNull(item);
            string newName = "NewName";
            string newPlacementNote = "NewPlacementNote";
            string newDescription = "NewDescription";
            int newQuantity = 10;
            item.Name = newName;
            item.PlacementNote = newPlacementNote;
            item.Description = newDescription;
            item.Quantity = newQuantity;
            await WaitUntilAsync(() => item.Item is not null && item.Item.Quantity == newQuantity);
            await WaitUntilAsync(() => item.Item is not null && item.Item.Name == newName);
            await WaitUntilAsync(() => item.Item is not null && item.Item.PlacementNote == newPlacementNote);
            await WaitUntilAsync(() => item.Item is not null && item.Item.Description == newDescription);
            Assert.Equal(newName, item.Item!.Name);
            Assert.Equal(newPlacementNote, item.Item!.PlacementNote);
            Assert.Equal(newDescription, item.Item!.Description);
            Assert.Equal(newQuantity, item.Item!.Quantity);
        }

        [Fact]
        public async Task UpdateNewItemInfoBeforeCreated()
        {
            var locationId = Guid.NewGuid();
            var vm = CreateRightPaneVM(out var _, out var _);
            await vm.LoadAsync(new(locationId, "location", null, 0));
            vm.AddNewItem();
            Assert.NotEmpty(vm.Items);
            var item = vm.Items[0];
            Assert.True(item.IsNew);
            Assert.Null(item.Item);
            string newName = "NewName";
            string newPlacementNote = "NewPlacementNote";
            string newDescription = "NewDescription";
            int newQuantity = 10;
            item.PlacementNote = newPlacementNote;
            item.Description = newDescription;
            item.Quantity = newQuantity;
            Assert.Null(item.Item);
            item.Name = "NewName";

            await WaitUntilAsync(() => item.Item is not null && item.Item.Name == "NewName");
            Assert.Equal(newName, item.Item!.Name);
            Assert.Equal(newPlacementNote, item.Item!.PlacementNote);
            Assert.Equal(newDescription, item.Item!.Description);
            Assert.Equal(newQuantity, item.Item!.Quantity);
        }

        [Fact]
        public async Task CreateNewItem_IsBusy_DontCreateNewItem()
        {
            var locationId = Guid.NewGuid();
            var guids = new List<Guid> {
                Guid.NewGuid()};
            var vm = CreateRightPaneVM(out var _, out var mockDialogService, guids, locationId);
            await vm.LoadAsync(new(locationId, "location", null, 0));
            Assert.NotEmpty(vm.Items);
            var item = vm.Items[0];
            vm.IsBusy = true;
            item.Name = "NewName";
            await WaitUntilAsync(() => mockDialogService.Messages.Count != 0);
            Assert.Equal("name", item.Item!.Name);
            Assert.Single(mockDialogService.Messages);
        }

        [Fact]
        public async Task AddNewItem_CalledTwice_DontAddNewItemShowsError()
        {
            var vm = CreateRightPaneVM(out var _, out var mockDialogService);
            await vm.LoadAsync(new(Guid.NewGuid(), "location", null, 0));
            vm.AddNewItem();
            vm.AddNewItem();
            Assert.Single(mockDialogService.Messages);
            Assert.Single(vm.Items);
        }

        [Fact]
        public async Task DeleteItem_ConfirmedYes_DeletesSelectedItems()
        {
            var guids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var locationId = Guid.NewGuid();
            var vm = CreateRightPaneVM(out var mockItemsService, out var mockDialogService, guids, locationId);
            mockDialogService.NextConfirmationResult = DialogResult.Yes;

            await vm.LoadAsync(new(locationId, "location", null, 0));
            vm.Items[0].IsSelected = true;
            vm.Items[1].IsSelected = true;
            await WaitUntilAsync(() => vm.DeleteItemCommand.CanExecute(null));

            await vm.DeleteItem();

            Assert.False(mockItemsService.Exists(guids[0]));
            Assert.False(mockItemsService.Exists(guids[1]));
            Assert.True(mockItemsService.Exists(guids[2]));
            Assert.Equal(2, mockItemsService.DeleteCalls);
            Assert.False(vm.IsBusy);
        }

        [Fact]
        public async Task DeleteItem_ConfirmationNo_DoesNotDeleteAnything()
        {
            var guids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var locationId = Guid.NewGuid();
            var vm = CreateRightPaneVM(out var mockItemsService, out var mockDialogService, guids, locationId);
            mockDialogService.NextConfirmationResult = DialogResult.No;

            await vm.LoadAsync(new(locationId, "location", null, 0));
            vm.Items[0].IsSelected = true;
            await WaitUntilAsync(() => vm.DeleteItemCommand.CanExecute(null));

            await vm.DeleteItem();

            Assert.True(mockItemsService.Exists(guids[0]));
            Assert.True(mockItemsService.Exists(guids[1]));
            Assert.Equal(0, mockItemsService.DeleteCalls);
            Assert.Equal(1, mockDialogService.ConfirmationDialogCalls);
        }

        [Fact]
        public async Task MoveToLocation_RaisesEvent_AndSetsSelectingFlag()
        {
            var guids = new List<Guid> { Guid.NewGuid() };
            var locationId = Guid.NewGuid();
            var vm = CreateRightPaneVM(out var _, out var _, guids, locationId);
            var eventRaised = false;
            vm.SelectNewLocationForItemsEvent += (_, __) => eventRaised = true;

            await vm.LoadAsync(new(locationId, "location", null, 0));
            vm.Items[0].IsSelected = true;
            await WaitUntilAsync(() => vm.MoveToLocationCommand.CanExecute(null));

            vm.MoveToLocation();

            Assert.True(eventRaised);
            Assert.True(vm.IsSelectingNewLocation);
        }

        [Fact]
        public async Task MoveSelectedItemsToLocation_UpdatesLocationForSelectedItems()
        {
            var guids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var sourceLocationId = Guid.NewGuid();
            var targetLocationId = Guid.NewGuid();
            var vm = CreateRightPaneVM(out var mockItemsService, out var _, guids, sourceLocationId);
            await vm.LoadAsync(new(sourceLocationId, "location", null, 0));

            vm.Items[0].IsSelected = true;
            vm.Items[2].IsSelected = true;
            await WaitUntilAsync(() => vm.MoveToLocationCommand.CanExecute(null));

            await vm.MoveSelectedItemsToLocation(new(targetLocationId, "target", null, 0));

            Assert.Equal(targetLocationId, mockItemsService.GetLocal(guids[0])!.LocationId);
            Assert.Equal(sourceLocationId, mockItemsService.GetLocal(guids[1])!.LocationId);
            Assert.Equal(targetLocationId, mockItemsService.GetLocal(guids[2])!.LocationId);
            Assert.Equal(2, mockItemsService.UpdateCalls);
            Assert.False(vm.IsBusy);
            Assert.False(vm.IsSelectingNewLocation);
        }

        [Fact]
        public async Task MoveSelectedItemsToLocation_NoSelection_ThrowsInvalidOperationException()
        {
            var locationId = Guid.NewGuid();
            var vm = CreateRightPaneVM(out var _, out var _);
            await vm.LoadAsync(new(locationId, "location", null, 0));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                vm.MoveSelectedItemsToLocation(new(Guid.NewGuid(), "target", null, 0)));
        }

        private static RightPaneViewModel CreateRightPaneVM(
            out MockItemsService mockItemsService,
            out MockDialogService mockDialogService,
            List<Guid>? guids = null,
            Guid? locationId = null)
        {
            MockLocationsService mockLocationsService = new();
            mockItemsService = new();
            mockDialogService = new();
            RightPaneViewModel vm = new(mockLocationsService, mockItemsService, mockDialogService, new ErrorLocalizerService());
            if (guids != null && locationId != null)
            {
                mockLocationsService.GenerateItems(guids, (Guid)locationId);
                mockItemsService.GenerateItems(guids, (Guid)locationId);
            }

            return vm;
        }
    }
}
