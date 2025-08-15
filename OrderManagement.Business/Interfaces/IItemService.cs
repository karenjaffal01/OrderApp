using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Business.Interfaces
{
    public interface IItemService
    {
        Task<Response<object>> CreateItemAsync(CreateItemDTO dto);
        Task<(int errorCode, string message)> DeleteItem(int itemId);
        Task<(int errorCode, string message)> UpdateItem(UpdateItemDTO dto);
        Task<IEnumerable<ItemDTO>> GetItems();
    }
}
