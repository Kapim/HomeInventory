using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HomeInventory.Client.Auth
{
    public interface IAuthApiClient
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
    }
}
