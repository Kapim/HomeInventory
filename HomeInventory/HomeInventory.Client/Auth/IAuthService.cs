using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using HomeInventory.Contracts;

namespace HomeInventory.Client.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(string username, string password, CancellationToken ct = default);
        Task LogoutAsync(CancellationToken ct = default);
        Task<string?> GetTokenAsync(CancellationToken ct = default);
    }
}
