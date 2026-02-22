using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.UseCases
{
    public interface IItemUseCase
    {
        public Task AddItem(string name, Guid ownerUserId, Guid locationId);

        public Task<Item?> GetItem(Guid itemId);
    }
}
