using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class OrderItem
    {
            public int Id { get; set; }

            [Required]
            public int OrderId { get; set; }

            [Required]
            [StringLength(255)]
            public string ProductName { get; set; }

            [Required]
            [Range(1, 100)]
            public int Quantity { get; set; }

            [Required]
            [Range(1, 100000)]
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice => Quantity * UnitPrice;
            public string? CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

            public string? UpdatedBy { get; set; }
            public DateTime? UpdatedDate { get; set; }

            public bool IsActive { get; set; } = true;
            public bool IsDeleted { get; set; } = false;
    }
}
