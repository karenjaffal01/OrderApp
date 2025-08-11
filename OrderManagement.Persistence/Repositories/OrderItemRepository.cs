using Dapper;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
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
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IDbConnection _connection;

        public OrderItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddOrderItemAsync(CreateOrderItemDTO dto, IDbTransaction transaction)
        {
            var parameters = new
            {
                p_order_id = dto.OrderId,
                p_product_name = dto.ProductName,
                p_quantity = dto.Quantity,
                p_unit_price = dto.UnitPrice
            };

            var sql = "SELECT add_order_item(@p_order_id, @p_product_name, @p_quantity, @p_unit_price)";

            await _connection.ExecuteAsync(sql, parameters);

            return 1;
        }

        public async Task<(int errorCode, string message)> UpdateOrderItemAsync(UpdateOrderItemDTO dto, IDbTransaction transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_id", dto.Id);
            parameters.Add("p_product_name", dto.ProductName);
            parameters.Add("p_quantity", dto.Quantity);
            parameters.Add("p_unit_price", dto.UnitPrice);
            parameters.Add("p_updated_by", dto.UpdatedBy);

            var result = await _connection.QueryFirstAsync<(int, string)>(
                "SELECT * FROM public.update_order_item(@p_id, @p_product_name, @p_quantity, @p_unit_price, @p_updated_by)",
                parameters,
                transaction
            );

            return result;
        }

        public async Task<(int errorCode, string message)> DeleteOrderItemAsync(int id, IDbTransaction transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_id", id);

            var result = await _connection.QueryFirstAsync<(int, string)>(
                "SELECT * FROM public.delete_order_item(@p_id)",
                parameters,
                transaction
            );

            return result;
        }
    }


}