using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto)
    {
        int newOrderId = await _orderService.CreateOrderAsync(dto);
        var response = new Response<object>
        {
            Message = "Order created successfully",
            Data = new { OrderId = newOrderId },
            ErrorCode = 0
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] updateOrderDTO dto)
    {
        dto.Id = id;
        var response = await _orderService.UpdateOrderAsync(dto);
        return Ok(response);
    }
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteOrder(int id)
    //{
    //    if (id <= 0)
    //    {
    //        return BadRequest(new Response<object>
    //        {
    //            Message = "Invalid order ID.",
    //            Data = null,
    //            ErrorCode = 1
    //        });
    //    }

    //    var response = await _orderService.DeleteOrderAsync(id);
    //    return Ok(response);
    //}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrders(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new Response<object>
            {
                ErrorCode = 1,
                Message = "Invalid order ID.",
                Data = null
            });
        }

        var response = await _orderService.DeleteOrdersAsync(id);
        return Ok(response);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        if (id <= 0)
            return BadRequest(new Response<OrderDTO>
            {
                ErrorCode = 1,
                Message = "Invalid order ID.",
                Data = null
            });

        var response = await _orderService.GetOrderByIdAsync(id);
        return Ok(response);
    }
    [HttpPost("PlaceOrderWithItems")]
    public async Task<IActionResult> PlaceOrderWithItems([FromBody] PlaceOrderWithItemsDTO dto)
    {
        var response = await _orderService.PlaceOrderWithItemsAsync(dto.Order, dto.OrderItems);
        return Ok(response);
    }
}
