using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IItemsApiClient
    {
        Task<IReadOnlyList<ItemDto>> SearchAsync(string name, CancellationToken ct);
        Task<ItemDto> CreateAsync(CreateItemRequestDto request, CancellationToken ct);
        Task<ItemDto> GetByIdAsync(Guid id, CancellationToken ct);
        Task<ItemDto> UpdateAsync(Guid id, UpdateItemRequestDto request, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
