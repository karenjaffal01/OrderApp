using OrderManagement.Domain.Common;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Business.Interfaces
{
    public interface ILoginService
    {
        Task<Response<object>> CreateUserAsync(Login dto);
        Task<Response<object>> GetUserAsync(string username, string password);
    }

}
