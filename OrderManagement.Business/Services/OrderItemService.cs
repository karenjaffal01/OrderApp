using Microsoft.Extensions.Logging;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Threading.Tasks;

namespace OrderManagement.Business.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderUnitOfWork _unitOfWork;
        private readonly ILogger<OrderItemService> _logger;

        public OrderItemService(IOrderUnitOfWork unitOfWork, ILogger<OrderItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Response<object>> AddOrderItemAsync(CreateOrderItemDTO dto)
        {
            _logger.LogInformation("Adding order item for OrderId: {OrderId}", dto.OrderId);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                int orderItemId = await _unitOfWork.OrderItems.AddOrderItemAsync(dto, _unitOfWork.Transaction);

                await _unitOfWork.CommitAsync();

                return new Response<object>
                {
                    Code = Response<object>.ErrorCode.Success,
                    Message = "Order item added successfully",
                    Data = new { ItemId = orderItemId }
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Failed to add order item for OrderId: {OrderId}", dto.OrderId);

                return new Response<object>
                {
                    Code = Response<object>.ErrorCode.Error,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task<Response<object>> UpdateOrderItemAsync(UpdateOrderItemDTO dto)
        {
            _logger.LogInformation("Updating order item {OrderItemId}", dto.Id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var (errorCodeInt, message) = await _unitOfWork.OrderItems.UpdateOrderItemAsync(dto, _unitOfWork.Transaction);

                var errorCode = errorCodeInt == 0 ? Response<object>.ErrorCode.Success : Response<object>.ErrorCode.Error;

                if (errorCode == Response<object>.ErrorCode.Error)
                {
                    _logger.LogWarning("Failed to update order item {OrderItemId}. Reason: {Message}", dto.Id, message);
                    await _unitOfWork.RollbackAsync();
                }
                else
                {
                    _logger.LogInformation("Order item {OrderItemId} updated successfully", dto.Id);
                    await _unitOfWork.CommitAsync();
                }

                return new Response<object>
                {
                    Message = message,
                    Data = new { ItemId = dto.Id },
                    Code = errorCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating order item {OrderItemId}", dto.Id);
                await _unitOfWork.RollbackAsync();
                return new Response<object>
                {
                    Message = ex.Message,
                    Data = null,
                    Code = Response<object>.ErrorCode.Error
                };
            }
        }

        public async Task<Response<object>> DeleteOrderItemAsync(int id)
        {
            _logger.LogInformation("Attempting to delete order item {OrderItemId}", id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var (errorCodeInt, message) = await _unitOfWork.OrderItems.DeleteOrderItemAsync(id, _unitOfWork.Transaction);

                var errorCode = errorCodeInt == 0 ? Response<object>.ErrorCode.Success : Response<object>.ErrorCode.Error;

                if (errorCode == Response<object>.ErrorCode.Error)
                {
                    _logger.LogWarning("Could not delete order item {OrderItemId}. Reason: {Message}", id, message);
                    await _unitOfWork.RollbackAsync();
                }
                else
                {
                    _logger.LogInformation("Order item {OrderItemId} deleted successfully", id);
                    await _unitOfWork.CommitAsync();
                }

                return new Response<object>
                {
                    Message = message,
                    Data = new { ItemId = id },
                    Code = errorCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting order item {OrderItemId}", id);
                await _unitOfWork.RollbackAsync();
                return new Response<object>
                {
                    Message = ex.Message,
                    Data = null,
                    Code = Response<object>.ErrorCode.Error
                };
            }
        }
    }
}
