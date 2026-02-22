using System;
using System.Collections.Generic;
using System.Text;
using HomeInventory.Contracts;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IItemsService
    {
        Task<IReadOnlyList<ItemDto>> SearchAsync(string query);
    }
}
