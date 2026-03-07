using HomeInventory.Client.Errors;
using HomeInventory.Client.Http;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;

namespace HomeInventory.Client.Tests
{
    public class HttpHouseholdsClientTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsHouseholds()
        {
            var expected = new List<HouseholdDto>
            {
                new(Guid.NewGuid(), "Family"),
                new(Guid.NewGuid(), "Weekend house")
            };

            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Get, "api/households", expected);

            var client = new HttpHouseholdsClient(api.CreateClient());
            var result = await client.GetAllAsync(CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.Contains("GET api/households", api.Requests);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedHousehold()
        {
            var expected = new HouseholdDto(Guid.NewGuid(), "Family");
            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Post, "api/households", expected);

            var client = new HttpHouseholdsClient(api.CreateClient());
            var result = await client.CreateAsync(new CreateHouseholdRequestDto("Family"), CancellationToken.None);

            Assert.Equal(expected, result);
            Assert.Contains("POST api/households", api.Requests);
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsHousehold()
        {
            var guid = Guid.NewGuid();
            var expected = new HouseholdDto(guid, "Family");
            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Get, $"api/households/{guid}", expected);

            var client = new HttpHouseholdsClient(api.CreateClient());
            var result = await client.GetByIdAsync(guid, CancellationToken.None);

            Assert.Equal(expected, result);
            Assert.Contains($"GET api/households/{guid}", api.Requests);
        }

        [Fact]
        public async Task GetByIdAsync_NotExistingId_ThrowsNotFoundApiException()
        {
            var guid = Guid.NewGuid();
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Get, $"api/households/{guid}", System.Net.HttpStatusCode.NotFound);

            var client = new HttpHouseholdsClient(api.CreateClient());
            try
            {
                var result = await client.GetByIdAsync(guid, CancellationToken.None);
            } catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.NotFound, ex.Type);
            }
        }

        [Fact]
        public async Task GetLocations_ReturnsLocationList()
        {
            var householdId = Guid.NewGuid();
            var expected = new List<LocationListItemDto>
            {
                new(Guid.NewGuid(), "Kitchen", LocationTypeDto.Room, null, 1)
            };

            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Get, $"api/households/{householdId}/locations", expected);

            var client = new HttpHouseholdsClient(api.CreateClient());
            var result = await client.GetLocations(householdId, CancellationToken.None);

            Assert.Single(result);
            Assert.Contains($"GET api/households/{householdId}/locations", api.Requests);
        }
    }
}
