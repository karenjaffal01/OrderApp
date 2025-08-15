using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.DTO;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IDbConnection _connection;
        public ItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<(int? itemId, int errorCode, string message)> CreateItem(CreateItemDTO dto)
        {
            var sql = "SELECT * FROM inventory.create_item(@item_name, @item_description, @item_category)";
            var result = await _connection.QuerySingleAsync<dynamic>(sql, new
            {
                item_name = dto.ItemName,
                item_description = dto.ItemDescription,
                item_category = dto.ItemCategory
            });

            return ((int?)result.new_item_id, (int)result.err_code, (string)result.msg);
        }


        public async Task<(int errorCode, string message)> DeleteItem(int itemId)
        {
            var sql = "SELECT * FROM inventory.delete_item(@item_id)";
            var result = await _connection.QueryFirstAsync<(int, string)>(sql, new { item_id = itemId });
            return result;
        }

        public async Task<(int errorCode, string message)> UpdateItem(UpdateItemDTO dto)
        {
            var sql = "SELECT * FROM inventory.update_item(@p_item_id, @p_item_name, @p_item_description, @p_item_category, @p_updated_by)";
            var result = await _connection.QueryFirstAsync<(int, string)>(sql, new
            {
                p_item_id = dto.ItemId,
                p_item_name = dto.ItemName,
                p_item_description = dto.ItemDescription,
                p_item_category = dto.ItemCategory,
                p_updated_by = dto.UpdatedBy
            });
            return result;
        }

        public async Task<IEnumerable<ItemDTO>> GetItems()
        {
            var sql = @"
            SELECT 
                item_id AS ""ItemId"",
                item_name AS ""ItemName"",
                item_description AS ""ItemDescription"",
                item_category AS ""ItemCategory"",
                created_date AS ""CreatedAt"",
                created_by AS ""CreatedBy""
            FROM inventory.items
            WHERE is_deleted = FALSE
            ORDER BY created_date DESC;
            ";

            var result = await _connection.QueryAsync<ItemDTO>(sql);

            return result;
        }

    }
}
