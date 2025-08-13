using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
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
        Task<Response<object>> RegisterAsync(LoginDTO dto);
        Task<string> LoginUser(LoginDTO user);
        string CreateToken(Login user);
    }
}
