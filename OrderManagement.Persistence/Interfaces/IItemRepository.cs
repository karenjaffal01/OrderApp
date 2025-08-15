using OrderManagement.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Interfaces
{
    public interface IItemRepository
    {
        Task<(int? itemId, int errorCode, string message)> CreateItem(CreateItemDTO dto);
        Task<(int errorCode, string message)> DeleteItem(int itemId);
        Task<(int errorCode, string message)> UpdateItem(UpdateItemDTO dto);
        Task<IEnumerable<ItemDTO>> GetItems();

    }
}
