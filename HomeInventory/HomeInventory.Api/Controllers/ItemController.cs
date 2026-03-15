using HomeInventory.Api.Mapping;
using HomeInventory.Application.Models;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Authentication;
using System.Security.Claims;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemController(IItemUseCase items) : ControllerBase
    {
        private readonly IItemUseCase _items = items;

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(Guid id)
        {
            var item = await _items.GetItemAsync(id, new CancellationTokenSource().Token);
            if (item == null)
            {
                return NotFound();
            } else
            {
                return Ok(ItemMapping.Map(item!));
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            try
            {
                await _items.DeleteItemAsync(id, new CancellationTokenSource().Token);
                return Ok();
            } catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> AddItem(CreateItemRequestDto createItemRequestDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();
            try
            {
                return ItemMapping.Map(await _items.AddItemAsync(ItemMapping.Map(createItemRequestDto), Guid.Parse(userIdClaim), new CancellationTokenSource().Token));
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is ArgumentException)
            {
                return BadRequest(((ArgumentException)ex).ParamName);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ItemDto>> UpdateItem(Guid id, UpdateItemRequestDto requestDto)
        {
            try
            {
                return Ok(ItemMapping.Map(await _items.UpdateItemAsync(id, ItemMapping.Map(requestDto), new CancellationTokenSource().Token)));
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is ArgumentException)
            {
                return BadRequest(((ArgumentException)ex).ParamName);
            }
        }

        
    }

    
}
