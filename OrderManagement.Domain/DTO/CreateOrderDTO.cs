using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.DTO
{
    public class CreateOrderDTO
    {
            public string CustomerName { get; set; }
            public string CreatedBy { get; set; }
    }
}
