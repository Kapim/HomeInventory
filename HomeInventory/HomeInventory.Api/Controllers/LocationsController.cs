using HomeInventory.Api.Mapping;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationsController(ILocationUseCase locations) : ControllerBase
    {
        private readonly ILocationUseCase _locations = locations;

        [HttpGet("{id}")]
        public async Task<LocationDto> GetLocation(string id)
        {
            return LocationMapping.Map(await _locations.GetLocationAsync(Guid.Parse(id)));
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
    }
}
