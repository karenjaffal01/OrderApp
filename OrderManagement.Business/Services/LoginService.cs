using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System.Security.Claims;
using System.Text;

public class LoginService : ILoginService
{
    private readonly ILoginRepository _repo;
    private readonly ILogger<LoginService> _logger;
    private readonly PasswordHasher<Login> _passwordHasher;

    public LoginService(ILoginRepository repo, ILogger<LoginService> logger)
    {
        _repo = repo;
        _logger = logger;
        _passwordHasher = new PasswordHasher<Login>();
    }

    public async Task<Response<object>> RegisterAsync(LoginDTO dto)
    {
        _logger.LogInformation("Attempting to register user with username: {Username}", dto.Username);

        try
        {
            var user = new Login
            {
                Username = dto.Username,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var result = await _repo.CreateUserAsync(user);

            if (result.Code == (int)Response<object>.ErrorCode.Success)
            {
                _logger.LogInformation("User '{Username}' registered successfully", dto.Username);
            }
            else
            {
                _logger.LogWarning("Failed to register user '{Username}'. Reason: {Message}", dto.Username, result.Message);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while registering user '{Username}'", dto.Username);
            return new Response<object>
            {
                Message = ex.Message,
                Data = null,
                Code = Response<object>.ErrorCode.Error
            };
        }
    }
    public string CreateToken(Login user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")));
        return "ok";
    }
}
