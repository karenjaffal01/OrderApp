using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<int> AddOrderItemAsync(CreateOrderItemDTO dto, IDbTransaction transaction);
        Task<(int errorCode, string message)> UpdateOrderItemAsync(UpdateOrderItemDTO dto, IDbTransaction transaction);
        Task<(int errorCode, string message)> DeleteOrderItemAsync(int id, IDbTransaction transaction);
    }
}
