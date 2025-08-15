using Dapper;
using OrderManagement.Persistence.Interfaces;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrderManagement.Persistence.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly IDbConnection _connection;

        public StockRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<(int errorCode, string message)> CreateStock(int itemId)
        {
            var sql = "SELECT * FROM inventory.create_stock(@item_id)";
            var result = await _connection.QuerySingleAsync<dynamic>(sql, new { item_id = itemId });
            return ((int)result.error_code, (string)result.message);
        }


        public async Task<(int errorCode, string message)> UpdateStockQuantity(int stockId, int quantity)
        {
            var sql = "SELECT * FROM inventory.update_stock_quantity(@stock_id,@quantity)";
            var result = await _connection.QuerySingleAsync<dynamic>(sql, new { stock_id = stockId, quantity });
            return ((int)result.error_code, (string)result.message);
        }

        public async Task<(int errorCode, string message)> DeleteStock(int stockId)
        {
            var sql = "SELECT * FROM inventory.delete_stock(@stock_id)";
            var result = await _connection.QuerySingleAsync<dynamic>(sql, new { stock_id = stockId });
            return ((int)result.error_code, (string)result.message);
        }

        public async Task<int> GetStockQuantity(int itemId)
        {
            var sql = "SELECT inventory.get_stock_quantity(@item_id)";
            var result = await _connection.QuerySingleOrDefaultAsync<int>(sql, new { item_id = itemId });
            return result;
        }

        public async Task<IEnumerable<dynamic>> GetAllStocks()
        {
            var sql = "SELECT * FROM inventory.get_all_stocks()";
            return await _connection.QueryAsync<dynamic>(sql);
        }
    }
}
