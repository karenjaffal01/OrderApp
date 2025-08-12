using Dapper;
using Npgsql;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System.Data;
using System.Threading.Tasks;

public class LoginRepository : ILoginRepository
{
    private readonly IDbConnection _connection;

    public LoginRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Response<object>> CreateUserAsync(Login user)
    {
        var sql = "SELECT public.create_user(@p_username, @p_password);";

        try
        {
            var newUserId = await _connection.QuerySingleAsync<int>(sql, new
            {
                p_username = user.Username, 
                p_password = user.PasswordHash
            });

            return new Response<object>
            {
                Message = "User registered successfully",
                Data = new { UserId = newUserId },
                Code = (int)Response<object>.ErrorCode.Success
            };
        }
        catch (PostgresException pgEx) when (pgEx.SqlState == "23505") // unique_violation
        {
            return new Response<object>
            {
                Message = "Username already exists",
                Data = null,
                Code = Response<object>.ErrorCode.Error
            };
        }
        catch (Exception ex)
        {
            return new Response<object>
            {
                Message = $"Error registering user: {ex.Message}",
                Data = null,
                Code = Response<object>.ErrorCode.Error
            };
        }
    }


}
