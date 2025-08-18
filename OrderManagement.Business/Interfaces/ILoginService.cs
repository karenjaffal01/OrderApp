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
        Task<TokenResponseDTO> LoginUser(LoginRequestDTO user);
        Task<TokenResponseDTO> RefreshTokensAsync(RefreshTokenRequestDTO request);
        string CreateToken(Login user);
    }
}
