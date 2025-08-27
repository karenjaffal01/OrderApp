using OrderManagement.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagement.Business.Interfaces
{
    public interface IStockService
    {
        Task<Response<object>> CreateStockAsync(int itemId);
        Task<Response<object>> UpdateStockQuantityAsync(int stockId, int quantity);
        Task<Response<object>> DeleteStockAsync(int stockId);
        Task<Response<int>> GetStockQuantityAsync(int itemId);
        Task<Response<IEnumerable<dynamic>>> GetAllStocksAsync();
    }
}
