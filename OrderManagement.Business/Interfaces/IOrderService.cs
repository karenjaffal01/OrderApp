using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Business.Interfaces
{
    public interface IOrderService
    {
        Task<Response<object>> CreateOrderAsync(CreateOrderDTO dto);
        Task<Response<object>> UpdateOrderAsync(updateOrderDTO dto);
        Task<Response<object>> DeleteOrderAsync(int orderId);
        Task<Response<object>> DeleteOrdersAsync(int orderId);
        Task<Response<OrderDTO>> GetOrderByIdAsync(int orderId);
        Task<Response<object>> PlaceOrderWithItemsAsync(CreateOrderDTO orderDto, List<CreateOrderItemDTO> itemsDto);
        Task<Response<List<OrderDTO>>> GetAllOrdersAsync();
    }
}