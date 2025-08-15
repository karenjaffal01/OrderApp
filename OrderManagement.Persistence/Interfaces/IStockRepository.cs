using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Interfaces
{
    public interface IStockRepository
    {
        Task<(int errorCode, string message)> CreateStock(int itemId);
        Task<(int errorCode, string message)> UpdateStockQuantity(int stockId, int quantity);
        Task<(int errorCode, string message)> DeleteStock(int stockId);
        Task<int> GetStockQuantity(int itemId);
        Task<IEnumerable<dynamic>> GetAllStocks();
    }
}
