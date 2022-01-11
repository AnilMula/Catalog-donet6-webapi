using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Collections.Generic;
using Catalog.Entities;
using Catalog.Dtos;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
           this. repository = repository;
        }

        [HttpGet]  // GET /items
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await repository.GetItemsAsync())
                        .Select( item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")] // GET /items/{id}
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }
        
        [HttpPost]      //POST /items
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id}, item.AsDto());
        }

        [HttpPut("{id}")]  // PUT /items/{id}
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await repository.GetItemAsync(id);
            if( existingItem is null)
            {
                return NotFound();
            }
            Item UpdatedItem = existingItem with { // with takes a copy with new things
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            await repository.UpdateItemAsync(UpdatedItem);
            return NoContent();
        }

        [HttpDelete("{id}")] // DELETE  /items/{id}
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);

            if(existingItem is null)
            {
                return NotFound();
            }
            await repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}