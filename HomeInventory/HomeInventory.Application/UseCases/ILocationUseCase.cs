using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.UseCases
{
    public interface ILocationUseCase
    {

        public Task<Location> GetLocationAsync(Guid id);

        public Task AddLocation(string name, Guid houseHoldId, Guid ownerUserId, Guid? parentLocationId = default, LocationType locationType = LocationType.Other);

        public Task<IReadOnlyList<Location>> FindByNameAsync(string name, Guid? parentId = null);

        public Task<IReadOnlyList<Location>> SearchAsync(string name, Guid? withinParentId = null, int limit = 50);
    }
}
