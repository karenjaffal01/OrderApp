using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Common
{
    public class Response<T> //representing a generic class that can work with any type T so its a consistent API wrapper for any kind of result
    {
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }
        public int ErrorCode { get; set; }
    }
}
