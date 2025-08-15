using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.DTO
{
    public class PlaceOrderWithItemsDTO
    {
        public CreateOrderDTO Order { get; set; }
        public List<CreateOrderItemDTO> OrderItems { get; set; }
    }
}
