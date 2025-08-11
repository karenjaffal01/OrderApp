using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.DTO
{
    public class OrderItemDTO
    {
        public int ItemId { get; set; } 
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemCreatedBy { get; set; }
        public DateTime ItemCreatedDate { get; set; }
        public string ItemUpdatedBy { get; set; }
        public DateTime? ItemUpdatedDate { get; set; }
        public bool ItemIsActive { get; set; }
        public bool ItemIsDeleted { get; set; }
    }

}
