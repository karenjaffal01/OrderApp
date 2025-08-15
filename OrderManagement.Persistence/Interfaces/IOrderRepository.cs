using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using System.Data;

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(CreateOrderDTO dto, IDbTransaction transaction);
    Task<(int errorCode, string message)> UpdateOrderAsync(updateOrderDTO dto, IDbTransaction transaction);
    Task<(int ErrorCode, string Message)> DeleteOrderAsync(int orderId, IDbTransaction transaction);
    Task<(int ErrorCode, string Message)> DeleteOrderWithItemsAsync(int orderId, IDbTransaction transaction);
    Task<OrderDTO> GetOrderWithItemsAsync(int orderId);
    Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
}

