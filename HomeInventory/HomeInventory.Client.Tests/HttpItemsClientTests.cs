using HomeInventory.Client.Errors;
using HomeInventory.Client.Http;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System.Net;

namespace HomeInventory.Client.Tests
{
    public class HttpItemsClientTests
    {
        [Fact]
        public async Task GetByIdAsync_CallsItemsEndpoint_AndReturnsItem()
        {
            var itemId = Guid.NewGuid();
            var expected = new ItemDto(itemId, "Flashlight", 3, Guid.NewGuid(), Guid.NewGuid(), "Top shelf", "LED");

            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Get, $"api/items/{itemId}", expected);

            var client = new HttpItemsClient(api.CreateClient());
            var result = await client.GetByIdAsync(itemId, CancellationToken.None);

            Assert.Equal(expected, result);
            Assert.Contains($"GET api/items/{itemId}", api.Requests);
        }

        [Fact]
        public async Task GetByIdAsync_NotExistingId_ThrowsNotFoundApiException()
        {
            var itemId = Guid.NewGuid();

            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Get, $"api/items/{itemId}", HttpStatusCode.NotFound);

            var client = new HttpItemsClient(api.CreateClient());
            try
            {
                var result = await client.GetByIdAsync(itemId, CancellationToken.None);
            } catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.NotFound, ex.Type);
            }
        }

        [Fact]
        public async Task DeleteAsync_CallsItemsEndpoint()
        {
            var itemId = Guid.NewGuid();
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Delete, $"api/items/{itemId}", HttpStatusCode.NoContent);

            var client = new HttpItemsClient(api.CreateClient());

            await client.DeleteAsync(itemId, CancellationToken.None);

            Assert.Contains($"DELETE api/items/{itemId}", api.Requests);
        }

        [Fact]
        public async Task DeleteAsync_NotExistingId_ThrowsNotFoundApiException()
        {
            var itemId = Guid.NewGuid();
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Delete, $"api/items/{itemId}", HttpStatusCode.NotFound);

            var client = new HttpItemsClient(api.CreateClient());
            try
            {
                await client.DeleteAsync(itemId, CancellationToken.None);
            } catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.NotFound, ex.Type);
            }           
        }

        [Fact]
        public async Task SearchAsync_EncodesQuery_AndReturnsMatches()
        {
            var expected = new List<ItemDto>
            {
                new(Guid.NewGuid(), "AA Battery", 8, Guid.NewGuid(), Guid.NewGuid(), null, null),
                new(Guid.NewGuid(), "AAA Battery", 4, Guid.NewGuid(), Guid.NewGuid(), null, null)
            };

            var api = new MockHomeInventoryApi();
            api.MapJson(HttpMethod.Get, "api/items/search?name=AA+battery", expected);

            var client = new HttpItemsClient(api.CreateClient());
            var result = await client.SearchAsync("AA battery", CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.Contains("GET api/items/search?name=AA+battery", api.Requests);
        }

        [Fact]
        public async Task CreateAsync_ThrowsApiUnavailableException_WhenTransportFails()
        {
            var api = new MockHomeInventoryApi();
            api.Map(HttpMethod.Post, "api/items", (_, _) => throw new HttpRequestException("Connection refused"));
            var client = new HttpItemsClient(api.CreateClient());

            var request = new CreateItemRequestDto("Box", 1, Guid.NewGuid(), null, null);
            try
            {
                await client.CreateAsync(request, CancellationToken.None);
            } catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.Network, ex.Type);
            }
        }

        [Fact]
        public async Task UpdateAsync_NotExistingId_ThrowsNotFoundApiException()
        {
            var itemId = Guid.NewGuid();

            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Put, $"api/items/{itemId}", HttpStatusCode.NotFound);
            UpdateItemRequestDto dto = new("NewName", null, 0, null, Guid.NewGuid());

            var client = new HttpItemsClient(api.CreateClient());
            try
            {
                var result = await client.UpdateAsync(itemId, dto, CancellationToken.None);
            }
            catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.NotFound, ex.Type);
            }
        }
    }
}
