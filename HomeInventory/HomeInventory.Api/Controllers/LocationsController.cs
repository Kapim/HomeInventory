using HomeInventory.Api.Mapping;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

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
            return LocationMapping.Map(await _locations.GetLocationAsync(Guid.Parse(id), new CancellationTokenSource().Token));
        }

        [HttpPatch("{id}")]
        public async Task<LocationDto> UpdateLocation(Guid id, UpdateLocationRequestDto request)
        {
            return LocationMapping.Map(await _locations.UpdateAsync(id, LocationMapping.Map(request), new CancellationTokenSource().Token));
        }

        [HttpPost]
        public async Task<ActionResult<LocationDto>> AddLocation(CreateLocationRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();
            return LocationMapping.Map(await _locations.AddLocationAsync(LocationMapping.Map(request, Guid.Parse(userIdClaim)), new CancellationTokenSource().Token));
        }

        [HttpGet("find")]
        public async Task<List<LocationDto>> FindByName(string name, Guid? parentId)
        {
            return [.. (await _locations.FindByNameAsync(name, parentId, new CancellationTokenSource().Token)).Select(x => LocationMapping.Map(x))];
        }

        [HttpGet("search")]
        public async Task<List<LocationDto>> Search(string name, Guid? withinParentId, int limit = 50)
        {
            return [.. (await _locations.SearchAsync(name, withinParentId, limit, new CancellationTokenSource().Token)).Select(x => LocationMapping.Map(x))];
        }

        [HttpGet("{id}/items")]
        public async Task<List<ItemDto>> GetItems(Guid id)
        {
            return [.. (await _locations.GetItemsAsync(id, new CancellationTokenSource().Token)).Select(x => ItemMapping.Map(x))];
        }
    }
}
