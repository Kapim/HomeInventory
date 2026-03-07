using HomeInventory.Client.Errors;
using HomeInventory.Client.Http;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System.Net;

namespace HomeInventory.Client.Tests
{
    public class HttpAuthClientTests
    {
        [Fact]
        public async Task LoginAsync_ReturnsResponse_WhenCredentialsAreValid()
        {
            var api = new MockHomeInventoryApi();
            var expected = new LoginResponseDto("token-123", UserRoleDto.Parent);
            api.MapJson(HttpMethod.Post, "api/auth/login", expected);

            var client = new HttpAuthClient(api.CreateClient());

            var result = await client.LoginAsync(new LoginRequestDto("john", "secret"));

            Assert.Equal("token-123", result.Token);
            Assert.Equal(UserRoleDto.Parent, result.UserRoleDto);
            Assert.Contains("POST api/auth/login", api.Requests);
        }

        [Fact]
        public async Task LoginAsync_ThrowsUnauthorizedApiException_WhenUnauthorized()
        {
            var api = new MockHomeInventoryApi();
            api.MapStatus(HttpMethod.Post, "api/auth/login", HttpStatusCode.Unauthorized);
            var client = new HttpAuthClient(api.CreateClient());
            try
            {
                await client.LoginAsync(new LoginRequestDto("john", "bad-password"));
            }
            catch (ApiException ex)
            {
                Assert.Equal(ApiErrorTypes.Unauthorized, ex.Type);
            }
        }
    }
}
