using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Business.Services;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;

namespace OrderManagement.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _inventoryService;

        public ItemController(IItemService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemDTO dto)
        {
            var response = await _inventoryService.CreateItemAsync(dto);

            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response); 
            else
                return BadRequest(response); 
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var (errorCode, message) = await _inventoryService.DeleteItem(id);

            if (errorCode == 0)
                return Ok(new { errorCode, message });
            else
                return BadRequest(new { errorCode, message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateItemDTO dto)
        {
            var (errorCode, message) = await _inventoryService.UpdateItem(dto);

            if (errorCode == 0)
                return Ok(new { errorCode, message });
            else
                return BadRequest(new { errorCode, message });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetItems()
        {
            var items = await _inventoryService.GetItems();

            if (items != null && items.Any())
                return Ok(items);
            else
                return NotFound("No items found.");
        }
    }

}
