using System;
using System.Collections.Generic;
using System.Text;
using HomeInventory.Contracts;

namespace HomeInventory.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string username, string password, CancellationToken ct = default);
        Task<bool> LogoutAsync();
        Task<bool> IsLoggedInAsync();
    }
}
