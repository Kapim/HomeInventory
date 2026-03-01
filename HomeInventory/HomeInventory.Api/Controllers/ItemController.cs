using HomeInventory.Api.Mapping;
using HomeInventory.Application.Models;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Authentication;

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
        public async Task<ItemDto> AddItem(CreateItemRequestDto createItemRequestDto)
        {
            return ItemMapping.Map(await _items.AddItemAsync(ItemMapping.Map(createItemRequestDto), new CancellationTokenSource().Token));
        }

        [HttpPatch("{id}")]
        public async Task<ItemDto> UpdateItem(Guid id, UpdateItemRequestDto requestDto)
        {
            return ItemMapping.Map(await _items.UpdateItemAsync(id, ItemMapping.Map(requestDto), new CancellationTokenSource().Token));
        }

        
    }

    
}
