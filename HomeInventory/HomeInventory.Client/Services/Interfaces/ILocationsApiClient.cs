using HomeInventory.Client.Requests;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface ILocationsApiClient
    {
        Task<LocationDto> GetByIdAsync(Guid id, CancellationToken ct);
        Task<LocationDto> UpdateAsync(Guid id, UpdateLocationRequestDto request, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<ItemDto>> GetItemsAsync(Guid locationId, CancellationToken ct);
        Task<LocationDto> CreateAsync(CreateLocationRequestDto request, CancellationToken ct);
    }
}
