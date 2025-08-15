using Microsoft.Extensions.Logging;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<Response<object>> CreateOrderAsync(CreateOrderDTO dto)
    {
        _logger.LogInformation("Starting creation of order for customer: {CustomerName}", dto.CustomerName);
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            int orderId = await _unitOfWork.Orders.CreateOrderAsync(dto, _unitOfWork.Transaction);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Order {OrderId} created successfully for customer: {CustomerName}", orderId, dto.CustomerName);

            return new Response<object>
            {
                Message = "Order created successfully",
                Data = new { OrderId = orderId },
                Code = Response<object>.ErrorCode.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for customer: {CustomerName}", dto.CustomerName);
            await _unitOfWork.RollbackAsync();

            return new Response<object>
            {
                Message = $"Failed to create order: {ex.Message}",
                Data = null,
                Code = Response<object>.ErrorCode.Error
            };
        }
    }



    public async Task<Response<object>> UpdateOrderAsync(updateOrderDTO dto)
    {
        _logger.LogInformation("Starting update for order {OrderId}", dto.Id);
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (errorCodeInt, message) = await _unitOfWork.Orders.UpdateOrderAsync(dto, _unitOfWork.Transaction);
            var errorCode = (Response<object>.ErrorCode)errorCodeInt;

            if (errorCode != Response<object>.ErrorCode.Success)
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
                Code = errorCode
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
                Code = Response<object>.ErrorCode.Error
            };
        }
    }

    public async Task<Response<object>> DeleteOrderAsync(int orderId)
    {
        _logger.LogInformation("Attempting to delete order {OrderId}", orderId);
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (errorCodeInt, message) = await _unitOfWork.Orders.DeleteOrderAsync(orderId, _unitOfWork.Transaction);
            var errorCode = (Response<object>.ErrorCode)errorCodeInt;

            if (errorCode != Response<object>.ErrorCode.Success)
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
                Code = errorCode
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
                Code = Response<object>.ErrorCode.Error
            };
        }
    }

    public async Task<Response<object>> DeleteOrdersAsync(int orderId)
    {
        _logger.LogInformation("Attempting to delete order {OrderId} and its related items", orderId);
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (errorCodeInt, message) = await _unitOfWork.Orders.DeleteOrderWithItemsAsync(orderId, _unitOfWork.Transaction);
            var errorCode = (Response<object>.ErrorCode)errorCodeInt;

            if (errorCode != Response<object>.ErrorCode.Success)
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
                Message = message,
                Data = null,
                Code = errorCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting order {OrderId} with items", orderId);
            await _unitOfWork.RollbackAsync();

            return new Response<object>
            {
                Message = ex.Message,
                Data = null,
                Code = Response<object>.ErrorCode.Error
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
                    Message = "Order not found",
                    Data = null,
                    Code = Response<OrderDTO>.ErrorCode.Error
                };
            }

            _logger.LogInformation("Order {OrderId} retrieved successfully", orderId);
            return new Response<OrderDTO>
            {
                Message = "Order retrieved successfully",
                Data = order,
                Code = Response<OrderDTO>.ErrorCode.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving order {OrderId}", orderId);

            return new Response<OrderDTO>
            {
                Message = ex.Message,
                Data = null,
                Code = Response<OrderDTO>.ErrorCode.Error
            };
        }
    }

    public async Task<Response<object>> PlaceOrderWithItemsAsync(CreateOrderDTO orderDto, List<CreateOrderItemDTO> itemsDto)
    {
        _logger.LogInformation("Placing order with items for customer: {CustomerName}", orderDto.CustomerName);
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

            _logger.LogInformation("Order {OrderId} and items created successfully", newOrderId);

            return new Response<object>
            {
                Message = "Order and items created successfully",
                Data = new { OrderId = newOrderId },
                Code = Response<object>.ErrorCode.Success
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            _logger.LogError(ex, "Failed to place order with items for customer: {CustomerName}", orderDto.CustomerName);

            return new Response<object>
            {
                Message = ex.Message,
                Data = null,
                Code = Response<object>.ErrorCode.Error
            };
        }
    }

    public async Task<Response<List<OrderDTO>>> GetAllOrdersAsync()
    {
        _logger.LogInformation("Retrieving all orders");

        try
        {
            var orders = await _unitOfWork.Orders.GetAllOrdersAsync();

            if (orders == null || !orders.Any())
            {
                return new Response<List<OrderDTO>>
                {
                    Message = "No orders found",
                    Data = null,
                    Code = Response<List<OrderDTO>>.ErrorCode.Error
                };
            }

            _logger.LogInformation("Orders retrieved successfully");

            return new Response<List<OrderDTO>>
            {
                Message = "Orders retrieved successfully",
                Data = orders.ToList(),
                Code = Response<List<OrderDTO>>.ErrorCode.Success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching orders");

            return new Response<List<OrderDTO>>
            {
                Message = $"An error occurred while fetching orders: {ex.Message}",
                Data = null,
                Code = Response<List<OrderDTO>>.ErrorCode.Error
            };
        }
    }
    
        

}
