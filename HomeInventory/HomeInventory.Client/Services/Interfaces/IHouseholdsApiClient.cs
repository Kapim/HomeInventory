using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IHouseholdsApiClient
    {
        Task<IReadOnlyList<HouseholdDto>> GetAllAsync(CancellationToken ct);
        Task<HouseholdDto> CreateAsync(string name);
    }
}
