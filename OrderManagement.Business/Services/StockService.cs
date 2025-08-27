using Microsoft.Extensions.Logging;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagement.Business.Services
{
    public class StockService : IStockService
    {
        private readonly ILogger<StockService> _logger;
        private readonly IStockUnitOfWork _unitOfWork;

        public StockService(ILogger<StockService> logger, IStockUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<object>> CreateStockAsync(int itemId)
        {
            _logger.LogInformation("Creating stock for item {ItemId}", itemId);

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var (errorCode, message) = await _unitOfWork.Stock.CreateStock(itemId);

                if (errorCode == 0)
                {
                    await _unitOfWork.CommitAsync();
                    return new Response<object>
                    {
                        Message = message ?? "Stock created successfully",
                        Data = new { ItemId = itemId },
                        Code = Response<object>.ErrorCode.Success
                    };
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    return new Response<object>
                    {
                        Message = message ?? "Failed to create stock",
                        Data = null,
                        Code = Response<object>.ErrorCode.Error
                    };
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating stock for item {ItemId}", itemId);
                return new Response<object>
                {
                    Message = ex.Message,
                    Data = null,
                    Code = Response<object>.ErrorCode.Error
                };
            }
        }

        public async Task<Response<object>> UpdateStockQuantityAsync(int stockId, int quantity)
        {
            _logger.LogInformation("Updating stock {StockId} quantity to {Quantity}", stockId, quantity);

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var (errorCode, message) = await _unitOfWork.Stock.UpdateStockQuantity(stockId, quantity);

                if (errorCode == 0)
                {
                    await _unitOfWork.CommitAsync();
                    return new Response<object>
                    {
                        Message = message ?? "Stock quantity updated successfully",
                        Data = new { StockId = stockId, Quantity = quantity },
                        Code = Response<object>.ErrorCode.Success
                    };
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    return new Response<object>
                    {
                        Message = message ?? "Failed to update stock quantity",
                        Data = null,
                        Code = Response<object>.ErrorCode.Error
                    };
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error updating stock {StockId}", stockId);
                return new Response<object>
                {
                    Message = ex.Message,
                    Data = null,
                    Code = Response<object>.ErrorCode.Error
                };
            }
        }

        public async Task<Response<object>> DeleteStockAsync(int stockId)
        {
            _logger.LogInformation("Deleting stock {StockId}", stockId);

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var (errorCode, message) = await _unitOfWork.Stock.DeleteStock(stockId);

                if (errorCode == 0)
                {
                    await _unitOfWork.CommitAsync();
                    return new Response<object>
                    {
                        Message = message ?? "Stock deleted successfully",
                        Data = null,
                        Code = Response<object>.ErrorCode.Success
                    };
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    return new Response<object>
                    {
                        Message = message ?? "Failed to delete stock",
                        Data = null,
                        Code = Response<object>.ErrorCode.Error
                    };
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error deleting stock {StockId}", stockId);
                return new Response<object>
                {
                    Message = ex.Message,
                    Data = null,
                    Code = Response<object>.ErrorCode.Error
                };
            }
        }

        public async Task<Response<int>> GetStockQuantityAsync(int itemId)
        {
            _logger.LogInformation("Retrieving stock quantity for item {ItemId}", itemId);

            try
            {
                int quantity = await _unitOfWork.Stock.GetStockQuantity(itemId);

                if (quantity >= 0)
                {
                    return new Response<int>
                    {
                        Message = "Stock quantity retrieved successfully",
                        Data = quantity,
                        Code = Response<int>.ErrorCode.Success
                    };
                }
                else
                {
                    return new Response<int>
                    {
                        Message = "Stock not found",
                        Data = -1,
                        Code = Response<int>.ErrorCode.Error
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock quantity for item {ItemId}", itemId);
                return new Response<int>
                {
                    Message = ex.Message,
                    Data = -1,
                    Code = Response<int>.ErrorCode.Error
                };
            }
        }

        public async Task<Response<IEnumerable<dynamic>>> GetAllStocksAsync()
        {
            _logger.LogInformation("Retrieving all stocks");

            try
            {
                var stocks = await _unitOfWork.Stock.GetAllStocks();

                if (stocks != null)
                {
                    return new Response<IEnumerable<dynamic>>
                    {
                        Message = "Stocks retrieved successfully",
                        Data = stocks,
                        Code = Response<IEnumerable<dynamic>>.ErrorCode.Success
                    };
                }
                else
                {
                    return new Response<IEnumerable<dynamic>>
                    {
                        Message = "No stocks found",
                        Data = null,
                        Code = Response<IEnumerable<dynamic>>.ErrorCode.Error
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all stocks");
                return new Response<IEnumerable<dynamic>>
                {
                    Message = ex.Message,
                    Data = null,
                    Code = Response<IEnumerable<dynamic>>.ErrorCode.Error
                };
            }
        }
    }
}
