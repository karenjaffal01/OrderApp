using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.DTO
{
    public class UpdateOrderItemDTO
    {
        [Required]
        public int Id { get; set; }

        [StringLength(255)]
        public string? ProductName { get; set; }

        [Range(1, 100)]
        public int? Quantity { get; set; }

        [Range(1, 100000)]
        public decimal? UnitPrice { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
