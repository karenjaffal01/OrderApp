using Microsoft.Extensions.Logging;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Threading.Tasks;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> CreateOrderAsync(CreateOrderDTO dto)
    {
        _logger.LogInformation("Starting creation of order for customer: {CustomerName}", dto.CustomerName);
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            int orderId = await _unitOfWork.Orders.CreateOrderAsync(dto, _unitOfWork.Transaction);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Order {OrderId} created successfully for customer: {CustomerName}", orderId, dto.CustomerName);
            return orderId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for customer: {CustomerName}", dto.CustomerName);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<Response<object>> UpdateOrderAsync(updateOrderDTO dto)
    {
        _logger.LogInformation("Starting update for order {OrderId}", dto.Id);
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var (errorCode, message) = await _unitOfWork.Orders.UpdateOrderAsync(dto, _unitOfWork.Transaction);
            if (errorCode != 0)
            {
                _logger.LogWarning("Update failed for order {OrderId}. Reason: {Message}", dto.Id, message);
                await _unitOfWork.RollbackAsync();
            }
            else
            {
                _logger.LogInformation("Order {OrderId} updated successfully", dto.Id);
                await _unitOfWork.CommitAsync();
            }

            return new Response<object>
            {
                Message = message,
                Data = new { OrderId = dto.Id },
                ErrorCode = errorCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating order {OrderId}", dto.Id);
            await _unitOfWork.RollbackAsync();
            return new Response<object>
            {
                Message = ex.Message,
                Data = null,
                ErrorCode = -1
            };
        }
    }

    public async Task<Response<object>> DeleteOrderAsync(int orderId)
    {
        _logger.LogInformation("Attempting to delete order {OrderId}", orderId);
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var (errorCode, message) = await _unitOfWork.Orders.DeleteOrderAsync(orderId, _unitOfWork.Transaction);

            if (errorCode != 0)
            {
                _logger.LogWarning("Order {OrderId} could not be deleted. Reason: {Message}", orderId, message);
                await _unitOfWork.RollbackAsync();
            }
            else
            {
                _logger.LogInformation("Order {OrderId} deleted successfully", orderId);
                await _unitOfWork.CommitAsync();
            }

            return new Response<object>
            {
                Message = message,
                Data = null,
                ErrorCode = errorCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting order {OrderId}", orderId);
            await _unitOfWork.RollbackAsync();
            return new Response<object>
            {
                Message = ex.Message,
                Data = null,
                ErrorCode = -1
            };
        }
    }

    public async Task<Response<object>> DeleteOrdersAsync(int orderId)
    {
        _logger.LogInformation("Attempting to delete order {OrderId} and its related items", orderId);
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (errorCode, message) = await _unitOfWork.Orders.DeleteOrderWithItemsAsync(orderId, _unitOfWork.Transaction);

            if (errorCode != 0)
            {
                _logger.LogWarning("Could not delete order {OrderId} with items. Reason: {Message}", orderId, message);
                await _unitOfWork.RollbackAsync();
            }
            else
            {
                _logger.LogInformation("Order {OrderId} and its items deleted successfully", orderId);
                await _unitOfWork.CommitAsync();
            }

            return new Response<object>
            {
                ErrorCode = errorCode,
                Message = message,
                Data = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting order {OrderId} with items", orderId);
            await _unitOfWork.RollbackAsync();
            return new Response<object>
            {
                ErrorCode = -1,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<Response<OrderDTO>> GetOrderByIdAsync(int orderId)
    {
        _logger.LogInformation("Retrieving order {OrderId} with items", orderId);
        try
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                return new Response<OrderDTO>
                {
                    ErrorCode = 1,
                    Message = "Order not found",
                    Data = null
                };
            }

            _logger.LogInformation("Order {OrderId} retrieved successfully", orderId);
            return new Response<OrderDTO>
            {
                ErrorCode = 0,
                Message = "Order retrieved successfully",
                Data = order
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving order {OrderId}", orderId);
            return new Response<OrderDTO>
            {
                ErrorCode = -1,
                Message = ex.Message,
                Data = null
            };
        }
    }
    public async Task<Response<object>> PlaceOrderWithItemsAsync(CreateOrderDTO orderDto, List<CreateOrderItemDTO> itemsDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            int newOrderId = await _unitOfWork.Orders.CreateOrderAsync(orderDto, _unitOfWork.Transaction);

            foreach (var item in itemsDto)
            {
                item.OrderId = newOrderId;
                await _unitOfWork.OrderItems.AddOrderItemAsync(item, _unitOfWork.Transaction);
            }

            await _unitOfWork.CommitAsync();

            return new Response<object>
            {
                Message = "Order and items created successfully",
                Data = new { OrderId = newOrderId },
                ErrorCode = 0
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            return new Response<object>
            {
                Message = ex.Message,
                Data = null,
                ErrorCode = -1
            };
        }
    }
}
