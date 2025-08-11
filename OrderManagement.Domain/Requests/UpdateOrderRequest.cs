using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Requests
{
    public class UpdateOrderRequest
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
