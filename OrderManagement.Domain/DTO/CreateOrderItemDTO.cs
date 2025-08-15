using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.DTO
{
    public class CreateOrderItemDTO
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }
        public int ItemId { get; set; }
        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        [Required]
        [Range(1, 100000)]
        public decimal UnitPrice { get; set; }

        public string? CreatedBy { get; set; }
    }

}
