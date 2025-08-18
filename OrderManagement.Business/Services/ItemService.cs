using Microsoft.Extensions.Logging;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Persistence.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace OrderManagement.Business.Services
{
    public class ItemService : IItemService
    {
        private readonly ILogger<ItemService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ItemService(ILogger<ItemService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<object>> CreateItemAsync(CreateItemDTO dto)
        {
            _logger.LogInformation("Creating new item: {ItemName}", dto.ItemName);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var (itemId, errorCode, message) = await _unitOfWork.Items.CreateItem(dto);

                if (errorCode != 0 || itemId == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new Response<object>
                    {
                        Code = Response<object>.ErrorCode.Error,
                        Message = message,
                        Data = null
                    };
                }

                var (stockError, stockMessage) = await _unitOfWork.Stock.CreateStock(itemId.Value);

                if (stockError != 0)
                {
                    await _unitOfWork.RollbackAsync();
                    return new Response<object>
                    {
                        Code = Response<object>.ErrorCode.Error,
                        Message = stockMessage,
                        Data = null
                    };
                }

                await _unitOfWork.CommitAsync();

                return new Response<object>
                {
                    Code = Response<object>.ErrorCode.Success,
                    Message = "Item and stock created successfully",
                    Data = new { ItemId = itemId, ItemName = dto.ItemName }
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Unexpected error while creating item and stock.");
                return new Response<object>
                {
                    Code = Response<object>.ErrorCode.Error,
                    Message = "An unexpected error occurred.",
                    Data = null
                };
            }
        }



        public async Task<(int errorCode, string message)> DeleteItem(int itemId)
        {
            _logger.LogInformation("Deleting item {ItemId}", itemId);
            return await _unitOfWork.Items.DeleteItem(itemId);
        }

        public async Task<(int errorCode, string message)> UpdateItem(UpdateItemDTO dto)
        {
            _logger.LogInformation("Updating item {ItemId} by {User}", dto.ItemId, dto.UpdatedBy);
            return await _unitOfWork.Items.UpdateItem(dto);
        }

        public async Task<IEnumerable<ItemDTO>> GetItems()
        {
            _logger.LogInformation("Fetching all items");
            return await _unitOfWork.Items.GetItems();
        }
    }
}
