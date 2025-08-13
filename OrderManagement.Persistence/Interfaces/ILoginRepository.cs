using OrderManagement.Domain.Common;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Interfaces
{
    public interface ILoginRepository
    {
        Task<Response<object>> CreateUserAsync(Login user);
        Task<Login?> GetUserAsync(string username);
    }
}
