using HomeInventory.Api.Mapping;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class TagsController(ITagUseCase tags, IItemUseCase items) : ControllerBase
    {
        [HttpGet("api/households/{householdId}/tags")]
        public async Task<ActionResult<IReadOnlyList<TagDto>>> GetTagsForHousehold(Guid householdId)
        {
            var result = await tags.GetTagsForHouseholdAsync(householdId);
            return Ok(result.Select(TagMapping.Map).ToList());
        }

        [HttpPost("api/tags")]
        public async Task<ActionResult<TagDto>> CreateTag(CreateTagRequestDto dto)
        {
            try
            {
                var tag = await tags.CreateTagAsync(TagMapping.Map(dto));
                return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, TagMapping.Map(tag));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is ArgumentOutOfRangeException)
            {
                return BadRequest(((ArgumentException)ex).ParamName);
            }
        }

        [HttpGet("api/tags/{id}")]
        public async Task<ActionResult<TagDto>> GetTag(Guid id)
        {
            var tag = await tags.GetTagAsync(id);
            return tag is null ? NotFound() : Ok(TagMapping.Map(tag));
        }

        [HttpDelete("api/tags/{id}")]
        public async Task<ActionResult> DeleteTag(Guid id)
        {
            try
            {
                await tags.DeleteTagAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("api/items/{itemId}/tags/{tagId}")]
        public async Task<ActionResult<ItemDto>> AssignTag(Guid itemId, Guid tagId)
        {
            try
            {
                var item = await tags.AssignTagToItemAsync(itemId, tagId);
                return Ok(ItemMapping.Map(item));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("api/items/{itemId}/tags/{tagId}")]
        public async Task<ActionResult<ItemDto>> RemoveTag(Guid itemId, Guid tagId)
        {
            try
            {
                var item = await tags.RemoveTagFromItemAsync(itemId, tagId);
                return Ok(ItemMapping.Map(item));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
