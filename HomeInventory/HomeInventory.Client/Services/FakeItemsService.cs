using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Services
{
    public class FakeItemsService : IItemsService
    {
        public Task<IReadOnlyList<ItemDto>> SearchAsync(string query)
        {
            var result = new List<ItemDto>();
            result.Add(new ItemDto());
            result.Add(new ItemDto());
            result.Add(new ItemDto());
            return Task.FromResult<IReadOnlyList<ItemDto>>(result);
        }
    }
}
