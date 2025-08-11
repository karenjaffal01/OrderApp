using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.DTO;

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
        [HttpPost]
        public async Task<IActionResult> AddOrderItem([FromBody] CreateOrderItemDTO dto)
        {
            var response = await _orderItemService.AddOrderItemAsync(dto);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] UpdateOrderItemDTO dto)
        {
            dto.Id = id;
            var response = await _orderItemService.UpdateOrderItemAsync(dto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var response = await _orderItemService.DeleteOrderItemAsync(id);
            return Ok(response);
        }

    }
}
