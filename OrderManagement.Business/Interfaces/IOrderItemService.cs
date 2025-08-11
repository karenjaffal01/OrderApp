using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Business.Interfaces
{
    public interface IOrderItemService
    {
        Task<Response<object>> AddOrderItemAsync(CreateOrderItemDTO dto);
        Task<Response<object>> UpdateOrderItemAsync(UpdateOrderItemDTO dto);
        Task<Response<object>> DeleteOrderItemAsync(int id);
    }
}
