using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Requests;
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
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        var dto = new CreateOrderDTO
        {
            CustomerName = request.CustomerName,
            CreatedBy = request.CreatedBy
        };

        var response = await _orderService.CreateOrderAsync(dto);

        if (response.Code == Response<object>.ErrorCode.Success)
            return Ok(response);

        return BadRequest(response);
    }
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(UpdateOrderRequest request)
    {
        var dto = new updateOrderDTO
        {
            Id = request.Id,
            CustomerName = request.CustomerName,
            UpdatedBy = request.UpdatedBy
        };

        var response = await _orderService.UpdateOrderAsync(dto);

        if (response.Code == Response<object>.ErrorCode.Success)
            return Ok(response);

        return BadRequest(response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrders(int id)
    {
        var response = await _orderService.DeleteOrdersAsync(id);

        if (response.Code == Response<object>.ErrorCode.Success)
            return Ok(response);

        return BadRequest(response);
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var response = await _orderService.GetOrderByIdAsync(id);

        if (response.Code == Response<OrderDTO>.ErrorCode.Success)
            return Ok(response);

        return BadRequest(response);
    }
    [Authorize]
    [HttpPost("PlaceOrderWithItems")]
    public async Task<IActionResult> PlaceOrderWithItems([FromBody] PlaceOrderWithItemsDTO dto)
    {
        var response = await _orderService.PlaceOrderWithItemsAsync(dto.Order, dto.OrderItems);

        if (response.Code == Response<object>.ErrorCode.Success)
            return Ok(response);

        return BadRequest(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var response = await _orderService.GetAllOrdersAsync();

        if (response.Code == Response<List<OrderDTO>>.ErrorCode.Success)
            return Ok(response);

        return BadRequest(response);
    }
}
