using HomeInventory.Api.Mapping;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController(ILocationUseCase locations) : ControllerBase
    {
        private readonly ILocationUseCase _locations = locations;

        [HttpGet("{id}")]
        public async Task<LocationDto> GetLocation(string id)
        {
            return LocationMapping.Map(await _locations.GetLocationAsync(Guid.Parse(id)));
        }

        [HttpPatch("{id}")]
        public async Task<LocationDto> UpdateLocation(Guid id, UpdateLocationRequestDto request)
        {
            return LocationMapping.Map(await _locations.UpdateAsync(id, LocationMapping.Map(request), new CancellationTokenSource().Token));
        }

        [HttpPost]
        public async Task AddLocation(string name, Guid householdId, Guid ownerUserId, Guid? parentLocationId = default, LocationTypeDto locationTypeDto = LocationTypeDto.Other)
        {
            await _locations.AddLocation(name, householdId, ownerUserId, parentLocationId, LocationTypeMapping.Map(locationTypeDto));
        }

        [HttpGet("find")]
        public async Task<List<LocationDto>> FindByName(string name, Guid? parentId)
        {
            return [.. (await _locations.FindByNameAsync(name, parentId)).Select(x => LocationMapping.Map(x))];
        }

        [HttpGet("search")]
        public async Task<List<LocationDto>> Search(string name, Guid? withinParentId, int limit = 50)
        {
            return [.. (await _locations.SearchAsync(name, withinParentId, limit)).Select(x => LocationMapping.Map(x))];
        }

        [HttpGet("{id}/items")]
        public async Task<List<ItemDto>> GetItems(Guid id)
        {
            return [.. (await _locations.GetItemsAsync(id, new CancellationTokenSource().Token)).Select(x => ItemMapping.Map(x))];
        }
    }
}
