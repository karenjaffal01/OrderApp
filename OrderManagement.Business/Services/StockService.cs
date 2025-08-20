using Microsoft.Extensions.Logging;
using OrderManagement.Business.Interfaces;
using OrderManagement.Persistence.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public async Task<(int errorCode, string message)> CreateStock(int itemId)
        {
            _logger.LogInformation("Creating stock for item {ItemId}", itemId);
            return await _unitOfWork.Stock.CreateStock(itemId);
        }

        public async Task<(int errorCode, string message)> UpdateStockQuantity(int stockId, int quantity)
        {
            _logger.LogInformation("Updating stock {StockId} quantity to {Quantity}", stockId, quantity);
            return await _unitOfWork.Stock.UpdateStockQuantity(stockId, quantity);
        }

        public async Task<(int errorCode, string message)> DeleteStock(int stockId)
        {
            _logger.LogInformation("Deleting stock {StockId}", stockId);
            return await _unitOfWork.Stock.DeleteStock(stockId);
        }

        public async Task<int> GetStockQuantity(int itemId)
        {
            return await _unitOfWork.Stock.GetStockQuantity(itemId);
        }

        public async Task<IEnumerable<dynamic>> GetAllStocks()
        {
            return await _unitOfWork.Stock.GetAllStocks();
        }
    }
}
