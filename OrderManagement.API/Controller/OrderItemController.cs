using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace OrderManagement.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddOrderItem([FromBody] CreateOrderItemDTO dto)
        {
            var response = await _orderItemService.AddOrderItemAsync(dto);
            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] UpdateOrderItemDTO dto)
        {
            dto.Id = id;
            var response = await _orderItemService.UpdateOrderItemAsync(dto);
            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var response = await _orderItemService.DeleteOrderItemAsync(id);
            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
