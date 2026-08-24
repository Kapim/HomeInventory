using HomeInventory.Api.Mapping;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

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

        [HttpGet("{id}/items")]
        public async Task<List<ItemDto>> GetItems(string id)
        {
            return [.. (await _households.GetItemsAsync(Guid.Parse(id))).Select(x => ItemMapping.Map(x))];
        }

        [HttpGet("{id}/search")]
        public async Task<ActionResult<IReadOnlyList<SearchResultDto>>> Search(Guid id, [FromQuery] string q, CancellationToken ct)
        {
            var results = await _households.SearchAsync(id, q ?? string.Empty, ct);
            return Ok(results.Select(r => new SearchResultDto(
                r.Id, r.Name, (SearchResultKindDto)r.Kind, r.LocationId, r.LocationPath, r.Description, r.TagMatch)).ToList());
        }

        [HttpGet("{id}/export")]
        public async Task<IActionResult> ExportHousehold(Guid id, CancellationToken ct)
        {
            var csv = await _households.ExportCsvAsync(id, ct);
            var bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"household_{id}_export.csv");
        }

        [HttpPost("{id}/import")]
        public async Task<ActionResult<ImportResultDto>> ImportHousehold(Guid id, IFormFile file, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest("Nebyl nahrán žádný soubor.");

            using var stream = file.OpenReadStream();
            var result = await _households.ImportCsvAsync(id, stream, Guid.Parse(userIdClaim), ct);
            return Ok(new ImportResultDto(result.LocationsImported, result.ItemsImported, result.Errors));
        }
    }


}
