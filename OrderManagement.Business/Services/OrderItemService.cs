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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderItemService> _logger;

        public OrderItemService(IUnitOfWork unitOfWork, ILogger<OrderItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> AddOrderItemAsync(CreateOrderItemDTO dto)
        {
            _logger.LogInformation("Starting to add order item for OrderId: {OrderId}", dto.OrderId);
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                int orderItemId = await _unitOfWork.OrderItems.AddOrderItemAsync(dto, _unitOfWork.Transaction);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Order item {OrderItemId} added successfully to OrderId: {OrderId}", orderItemId, dto.OrderId);
                return orderItemId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add order item for OrderId: {OrderId}", dto.OrderId);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Response<object>> UpdateOrderItemAsync(UpdateOrderItemDTO dto)
        {
            _logger.LogInformation("Updating order item {OrderItemId}", dto.Id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var (errorCode, message) = await _unitOfWork.OrderItems.UpdateOrderItemAsync(dto, _unitOfWork.Transaction);
                if (errorCode != 0)
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
                    ErrorCode = errorCode
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
                    ErrorCode = -1
                };
            }
        }

        public async Task<Response<object>> DeleteOrderItemAsync(int id)
        {
            _logger.LogInformation("Attempting to delete order item {OrderItemId}", id);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var (errorCode, message) = await _unitOfWork.OrderItems.DeleteOrderItemAsync(id, _unitOfWork.Transaction);
                if (errorCode != 0)
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
                    ErrorCode = errorCode
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
                    ErrorCode = -1
                };
            }
        }
    }
}
