using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Services
{
    public class FakeAuthService : IAuthService
    {
        private bool _isLoggedIn = false;
        public Task<bool> IsLoggedInAsync()
        {
            return Task.FromResult(_isLoggedIn);
        }

        public async Task<LoginResult> LoginAsync(string username, string password, CancellationToken ct = default)
        {
            await Task.Delay(500, ct);
            _isLoggedIn = true;
            return new(Guid.NewGuid(), UserRoleDto.Parent);
        }

        public async Task<bool> LogoutAsync()
        {
            await Task.Delay(500);
            if (!_isLoggedIn)
                return false;
            _isLoggedIn = false;
            return true;
        }
    }
}
