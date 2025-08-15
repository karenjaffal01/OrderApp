using Dapper;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using System.Data;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class OrderRepository : IOrderRepository
{
    private readonly IDbConnection _connection; //this field holds the database connection instance

    public OrderRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    //using function
    public async Task<int> CreateOrderAsync(CreateOrderDTO dto, IDbTransaction transaction)//to allow to pass transactions to use 
    {
        var sql = "SELECT public.create_order(@CustomerName, @OrderDate, @CreatedBy)"; //this is a db function 
        int newOrderId = await _connection.QuerySingleAsync<int>(sql, new //dapper call
        {
            CustomerName = dto.CustomerName,
            OrderDate = DateTime.UtcNow,
            CreatedBy = dto.CreatedBy
        }, transaction); //we pass transaction to execute the command on that transaction ensure its part of existing transaction instead of creating one

         return newOrderId;
    }

    public async Task<(int errorCode, string message)> UpdateOrderAsync(updateOrderDTO dto, IDbTransaction transaction)
    {
        var sql = "SELECT * FROM public.update_order(@p_order_id, @p_customer_name, @p_order_date, @p_updated_by);";

        var result = await _connection.QuerySingleAsync<(int errorCode, string message)>(
            sql,
            new
            {
                p_order_id = dto.Id,
                p_customer_name = dto.CustomerName,
                p_order_date = DateTime.UtcNow,
                p_updated_by = dto.UpdatedBy
            },
            transaction
        );

        return result;
    }
    public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
    {
        var sql = "SELECT * FROM public.get_all_orders();";

        var orderDict = new Dictionary<int, OrderDTO>();

        var result = await _connection.QueryAsync<OrderDTO, OrderItemDTO, OrderDTO>(
            sql,
            (order, item) =>
            {
                if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                {
                    currentOrder = order;
                    currentOrder.OrderItems = new List<OrderItemDTO>();
                    orderDict.Add(currentOrder.Id, currentOrder);
                }

                if (item != null && item.ItemId != 0)
                    currentOrder.OrderItems.Add(item);

                return currentOrder;
            },
            splitOn: "ItemId"
        );

        return orderDict.Values;
    }

    public async Task<(int ErrorCode, string Message)> DeleteOrderAsync(int orderId, IDbTransaction transaction)
    {
        var sql = "SELECT * FROM public.delete_order(@p_order_id);";

        var result = await _connection.QuerySingleAsync<(int ErrorCode, string Message)>(
            sql,
            new { p_order_id = orderId },
            transaction
        );

        return result;
    }
    public async Task<(int ErrorCode, string Message)> DeleteOrderWithItemsAsync(int orderId, IDbTransaction transaction)
    {
        var parameters = new { p_order_id = orderId };

        var result = await _connection.QuerySingleAsync<(int ErrorCode, string Message)>(
            "SELECT * FROM delete_order_with_items(@p_order_id)",
            parameters,
            transaction
        );

        return result;
    }

    public async Task<OrderDTO> GetOrderWithItemsAsync(int orderId)
    {
        var parameters = new { p_order_id = orderId };

        var orderDict = new Dictionary<int, OrderDTO>();

        var result = await _connection.QueryAsync<OrderDTO, OrderItemDTO, OrderDTO>(
    "SELECT * FROM get_order_with_items(@p_order_id)",
    (order, item) =>
    {
        if (!orderDict.TryGetValue(order.Id, out var currentOrder))
        {
            currentOrder = order;
            currentOrder.OrderItems = new List<OrderItemDTO>();
            orderDict.Add(currentOrder.Id, currentOrder);
        }

        if (item != null && item.ItemId != 0)
            currentOrder.OrderItems.Add(item);

        return currentOrder;
    },
    parameters,
    splitOn: "ItemId"
    );

        return orderDict.Values.FirstOrDefault();
    }


    //using procedure 
    /*public async Task<(int? OrderId, int ErrorCode, string Message)> CreateOrderAsync(CreateOrderDTO dto, IDbTransaction transaction)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_customer_name", dto.CustomerName);
        parameters.Add("p_order_date", dto.OrderDate);
        parameters.Add("p_created_by", dto.CreatedBy);
        parameters.Add("p_error_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("p_message", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
        parameters.Add("p_order_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _connection.ExecuteAsync(
            "CALL public.create_order(@p_customer_name, @p_order_date, @p_created_by, @p_error_code, @p_message, @p_order_id)",
            parameters,
            transaction
        );

        return (
            parameters.Get<int?>("p_order_id"),
            parameters.Get<int>("p_error_code"),
            parameters.Get<string>("p_message")
        );
    }*/

}
