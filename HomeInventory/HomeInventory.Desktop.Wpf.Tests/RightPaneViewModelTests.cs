using HomeInventory.Client.Models;
using HomeInventory.Client.Services;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.ViewModels;
using HomeInventory.Wpf.Tests;
using System.Windows.Controls;

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
            var item = vm.Items.Where(x => x.Item?.Id == guids[0]).FirstOrDefault();
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
            var vm = CreateRightPaneVM(out var mockItemsService, out var mockDialogService, guids, locationId);
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

        private static RightPaneViewModel CreateRightPaneVM(out MockItemsService mockItemsService, out MockDialogService mockDialogService, List<Guid>? guids = null, Guid? locationId = null)
        {
            MockLocationsService mockLocationsService = new();
            mockItemsService = new();
            mockDialogService = new();
            RightPaneViewModel vm = new(mockLocationsService, mockItemsService, mockDialogService, new ErrorLocalizerService());
            if (guids != null && locationId != null) { 
                mockLocationsService.GenerateItems(guids, (Guid)locationId);
                mockItemsService.GenerateItems(guids, (Guid)locationId);
            }

            return vm;
        }


    }
}
