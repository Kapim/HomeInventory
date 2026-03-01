using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IHouseholdsApiClient
    {
        Task<HouseholdDto> GetByIdAsync(Guid householdId, CancellationToken ct);
        Task<IReadOnlyList<HouseholdDto>> GetAllAsync(CancellationToken ct);
        Task<HouseholdDto> CreateAsync(CreateHouseholdRequestDto request, CancellationToken ct);
        Task<IReadOnlyList<LocationListItemDto>> GetLocations(Guid householdId, CancellationToken ct);
        Task<IReadOnlyList<ItemDto>> GetItems(Guid householdId, CancellationToken ct);
    }
}
