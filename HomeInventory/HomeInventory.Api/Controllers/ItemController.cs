using HomeInventory.Api.Mapping;
using HomeInventory.Application.Models;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
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
                return ItemMapping.Map(item!);
            }

        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> AddItem(CreateItemRequestDto createItemRequestDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
                return Unauthorized();
            return ItemMapping.Map(await _items.AddItemAsync(ItemMapping.Map(createItemRequestDto), Guid.Parse(userIdClaim), new CancellationTokenSource().Token));
        }

        [HttpPatch("{id}")]
        public async Task<ItemDto> UpdateItem(Guid id, UpdateItemRequestDto requestDto)
        {
            return ItemMapping.Map(await _items.UpdateItemAsync(id, ItemMapping.Map(requestDto), new CancellationTokenSource().Token));
        }

        
    }

    
}
