using HomeInventory.Api.Mapping;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Authentication;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/households")]
    [Authorize]
    public class HouseholdController(IHouseholdUseCase households) : ControllerBase
    {
        private readonly IHouseholdUseCase _households = households;

        [HttpGet("{id}")]
        public async Task<HouseholdDto> GetHousehold(string id)
        {
            return HouseholdMapping.Map(await _households.GetHouseholdAsync(Guid.Parse(id)));
        }

        [HttpPost]
        public async Task AddHousehold(string name)
        {
            await _households.AddHouseholdAsync(name);
        }

        [HttpGet]
        public async Task<List<HouseholdDto>> GetHouseholds()
        {
            return [.. (await _households.GetHouseholdsAsync()).Select(x => HouseholdMapping.Map(x))];
        }

        [HttpGet("{id}/locations")]
        public async Task<List<LocationListItemDto>> GetLocations(string id)
        {
            return [.. (await _households.GetLocationsAsync(Guid.Parse(id))).Select(x => LocationMapping.MapListItem(x))];
        }
    }

    
}
