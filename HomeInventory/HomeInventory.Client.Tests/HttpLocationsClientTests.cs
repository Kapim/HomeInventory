using HomeInventory.Client.Errors;
using HomeInventory.Client.Http;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System.Net;

namespace HomeInventory.Client.Tests
{
    public class HttpLocationsClientTests
    {
        [Fact]
        public async Task GetByIdAsync_CallsExpectedEndpoint_AndReturnsLocation()
        {
            var locationId = Guid.NewGuid();
            var expected = new LocationDto(locationId, "Garage", LocationTypeDto.Room, null, Guid.NewGuid(), Guid.NewGuid(), 1, "Main room");

            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Get, $"api/locations/{locationId}", expected);

            var client = new HttpLocationsClient(api.CreateClient());
            var result = await client.GetByIdAsync(locationId, CancellationToken.None);

            Assert.Equal(expected, result);
            Assert.Contains($"GET api/locations/{locationId}", api.Requests);
        }

        [Fact]
        public async Task GetByIdAsync_NotExistingId_ThrowsNotFoundApiException()
        {
            var locationId = Guid.NewGuid();
            
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Get, $"api/locations/{locationId}", HttpStatusCode.NotFound);

            var client = new HttpLocationsClient(api.CreateClient());
            try
            {
                var result = await client.GetByIdAsync(locationId, CancellationToken.None);
            } catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.NotFound, ex.Type);
            }

        }

        [Fact]
        public async Task CreateAsync_ThrowsUnauthorizedApiException_WhenUnauthorized()
        {
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Post, "api/locations", HttpStatusCode.Unauthorized);

            var client = new HttpLocationsClient(api.CreateClient());
            var request = new CreateLocationRequestDto("Garage", LocationTypeDto.Room, null, 1, null, Guid.NewGuid());
            try
            {
                await client.CreateAsync(request, CancellationToken.None);
            } catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.Unauthorized, ex.Type);
            }
        }

        [Fact]
        public async Task GetItemsAsync_ReturnsItemsForLocation()
        {
            var locationId = Guid.NewGuid();
            var expected = new List<ItemDto>
            {
                new(Guid.NewGuid(), "Hammer", 1, locationId, Guid.NewGuid(), null, null),
                new(Guid.NewGuid(), "Screwdriver", 2, locationId, Guid.NewGuid(), null, null)
            };

            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Get, $"api/locations/{locationId}/items", expected);

            var client = new HttpLocationsClient(api.CreateClient());
            var result = await client.GetItemsAsync(locationId, CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.Equal("Hammer", result[0].Name);
            Assert.Contains($"GET api/locations/{locationId}/items", api.Requests);
        }

        [Fact]
        public async Task DeleteAsync_CallsLocationsEndpoint()
        {
            var locationId = Guid.NewGuid();
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Delete, $"api/locations/{locationId}", HttpStatusCode.NoContent);

            var client = new HttpLocationsClient(api.CreateClient());

            await client.DeleteAsync(locationId, CancellationToken.None);

            Assert.Contains($"DELETE api/locations/{locationId}", api.Requests);
        }

        [Fact]
        public async Task DeleteAsync_NotExistingId_ThrowsNotFoundApiException()
        {
            var locationId = Guid.NewGuid();
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Delete, $"api/locations/{locationId}", HttpStatusCode.NotFound);

            var client = new HttpLocationsClient(api.CreateClient());
            try
            {
                await client.DeleteAsync(locationId, CancellationToken.None);
            }
            catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.NotFound, ex.Type);
            }
        }

        [Fact]
        public async Task UpdateAsync_NotExistingId_ThrowsNotFoundApiException()
        {
            var locationId = Guid.NewGuid();

            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Put, $"api/locations/{locationId}", HttpStatusCode.NotFound);
            UpdateLocationRequestDto dto = new("NewName", LocationTypeDto.Room, 0, null, Guid.NewGuid());

            var client = new HttpLocationsClient(api.CreateClient());
            try
            {
                var result = await client.UpdateAsync(locationId, dto, CancellationToken.None);
            }
            catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.NotFound, ex.Type);
            }
        }
    }
}
