using HomeInventory.Api.Mapping;
using HomeInventory.Application.UseCases;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Authentication;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/item")]
    public class ItemController(IItemUseCase items) : ControllerBase
    {
        private readonly IItemUseCase _items = items;

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(Guid id)
        {
            var item = await _items.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }               
            
            return ItemMapping.Map(item!);
        }

        [HttpPost]
        public async Task AddItem(string name, Guid ownerId, Guid locationId)
        {
            await _items.AddItem(name, ownerId, locationId);
        }

        
    }

    
}
