using HomeInventory.Client.Auth;
using HomeInventory.Client.Models;
using HomeInventory.Contracts;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using HomeInventory.Desktop.Wpf.ViewModels;
using HomeInventory.Wpf.Tests;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class ItemsSearchViewModelTests
    {
        private readonly Guid _householdId = Guid.NewGuid();
        private readonly Guid _locationId = Guid.NewGuid();

        private ItemsSearchViewModel CreateSut(MockHouseholdsService? households = null)
        {
            households ??= new MockHouseholdsService();
            return new ItemsSearchViewModel(
                new NullNavigationService(),
                new NullAuthService(),
                households,
                new MockDialogService());
        }

        private MockHouseholdsService BuildHouseholds(
            IEnumerable<Item>? items = null,
            IEnumerable<LocationListItem>? locations = null)
        {
            var svc = new MockHouseholdsService();
            svc.AddHousehold(new Household(_householdId, "Test household"));
            svc.SetItems(items ?? []);
            svc.SetLocations(locations ?? []);
            return svc;
        }

        private Item MakeItem(string name, Guid? locationId = null)
            => new(Guid.NewGuid(), name, 1, locationId ?? _locationId, Guid.NewGuid(), null, null);

        private LocationListItem MakeLocation(string name, Guid? parentId = null)
            => new(Guid.NewGuid(), name, LocationType.Other, parentId, 0);

        [Fact]
        public async Task EmptyQuery_NoResults()
        {
            var households = BuildHouseholds(
                items: [MakeItem("Šroubovák")],
                locations: [MakeLocation("Garáž")]
            );
            var vm = CreateSut(households);
            await vm.InitializeAsync();
            await Task.Delay(50);

            vm.Query = "";
            vm.SearchCommand.Execute(null);

            Assert.Empty(vm.Results);
        }

        [Fact]
        public async Task QueryMatchingItemName_ReturnsItemResult()
        {
            var households = BuildHouseholds(
                items: [MakeItem("Šroubovák"), MakeItem("Kladivo")]
            );
            var vm = CreateSut(households);
            await vm.InitializeAsync();
            await Task.Delay(50);

            vm.Query = "šrou";
            vm.SearchCommand.Execute(null);

            Assert.Single(vm.Results);
            Assert.Equal("Šroubovák", vm.Results[0].Name);
            Assert.Equal(SearchResultType.Item, vm.Results[0].Type);
        }

        [Fact]
        public async Task QueryMatchingLocationName_ReturnsLocationResult()
        {
            var households = BuildHouseholds(
                locations: [MakeLocation("Garáž"), MakeLocation("Obývák")]
            );
            var vm = CreateSut(households);
            await vm.InitializeAsync();
            await Task.Delay(50);

            vm.Query = "garáž";
            vm.SearchCommand.Execute(null);

            Assert.Single(vm.Results);
            Assert.Equal("Garáž", vm.Results[0].Name);
            Assert.Equal(SearchResultType.Location, vm.Results[0].Type);
        }

        [Fact]
        public async Task QueryMatchingBoth_ReturnsMixed()
        {
            var households = BuildHouseholds(
                items: [MakeItem("Polička")],
                locations: [MakeLocation("Polička")]
            );
            var vm = CreateSut(households);
            await vm.InitializeAsync();
            await Task.Delay(50);

            vm.Query = "polič";
            vm.SearchCommand.Execute(null);

            Assert.Equal(2, vm.Results.Count);
            Assert.Contains(vm.Results, r => r.Type == SearchResultType.Item);
            Assert.Contains(vm.Results, r => r.Type == SearchResultType.Location);
        }

        [Fact]
        public async Task ItemResult_ContainsLocationPath()
        {
            var locId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var households = BuildHouseholds(
                items: [new Item(Guid.NewGuid(), "Šroubovák", 1, locId, Guid.NewGuid(), null, null)],
                locations: [
                    new LocationListItem(parentId, "Garáž", LocationType.Room, null, 0),
                    new LocationListItem(locId, "Skříň", LocationType.Other, parentId, 0)
                ]
            );
            var vm = CreateSut(households);
            await vm.InitializeAsync();
            await Task.Delay(50);

            vm.Query = "šroubovák";
            vm.SearchCommand.Execute(null);

            Assert.Single(vm.Results);
            Assert.Equal("Garáž › Skříň", vm.Results[0].LocationPath);
        }

        [Fact]
        public async Task SearchIsCaseInsensitive()
        {
            var households = BuildHouseholds(items: [MakeItem("Šroubovák")]);
            var vm = CreateSut(households);
            await vm.InitializeAsync();
            await Task.Delay(50);

            vm.Query = "ŠROUBOVÁK";
            vm.SearchCommand.Execute(null);

            Assert.Single(vm.Results);
        }

        [Fact]
        public async Task HouseholdSelector_LoadedOnInit()
        {
            var households = BuildHouseholds();
            var vm = CreateSut(households);
            await vm.InitializeAsync();

            Assert.Single(vm.Households);
            Assert.Equal(_householdId, vm.SelectedHousehold!.Id);
        }

        // --- Minimal stub implementations ---

        private sealed class NullNavigationService : INavigationService
        {
            public object? CurrentViewModel => null;
            public event Action? CurrentViewModelChanged { add { } remove { } }
            public Task NavigateTo<TViewModel>() where TViewModel : class => Task.CompletedTask;
        }

        private sealed class NullAuthService : IAuthService
        {
            public Task<LoginResponseDto> LoginAsync(string u, string p, CancellationToken ct = default) => throw new NotImplementedException();
            public Task LogoutAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task<string?> GetTokenAsync(CancellationToken ct = default) => Task.FromResult<string?>(null);
        }
    }
}
