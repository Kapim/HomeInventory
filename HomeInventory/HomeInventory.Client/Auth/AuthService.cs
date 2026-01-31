using HomeInventory.Contracts;

namespace HomeInventory.Client.Auth
{
    public class AuthService(IAuthApiClient api, ITokenStore tokens) : IAuthService
    {
        private readonly IAuthApiClient _api = api;
        private readonly ITokenStore _token = tokens;

        public Task<string?> GetTokenAsync(CancellationToken ct = default)
        {
            return _token.LoadAsync(ct);
        }

        public async Task<LoginResponseDto> LoginAsync(string username, string password, CancellationToken ct = default)
        {
            var request = new LoginRequestDto(username, password);
            
            var response = await _api.LoginAsync(request, ct);
            await _token.SaveAsync(response.Token, ct);
            return response;
           
        }

        public Task LogoutAsync(CancellationToken ct = default)
        {
            return _token.ClearAsync(ct);
        }
    }
}