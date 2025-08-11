using Dapper;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IDbConnection _connection;

        public LoginRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Response<object>> CreateUserAsync(Login dto)
        {
            var sql = "SELECT * FROM create_user(@p_username, @p_password)";
            var result = await _connection.QueryFirstOrDefaultAsync<Response<object>>(sql, new
            {
                p_username = dto.Username,
                p_password = dto.Password
            });
            return result;
        }

        public async Task<Response<object>> GetUserAsync(string username, string password)
        {
            var sql = "SELECT * FROM get_user(@p_username, @p_password)";
            var result = await _connection.QueryFirstOrDefaultAsync<Response<object>>(sql, new
            {
                p_username = username,
                p_password = password
            });
            return result;
        }
    }
}
