using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class LoginService : ILoginService
{
    private readonly ILoginRepository _repo;
    private readonly ILogger<LoginService> _logger;
    private readonly PasswordHasher<Login> _passwordHasher;
    private readonly IConfiguration _configuration;
    public LoginService(ILoginRepository repo, ILogger<LoginService> logger, IConfiguration configuration)
    {
        _repo = repo;
        _logger = logger;
        _passwordHasher = new PasswordHasher<Login>();
        _configuration = configuration;
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

            user.Password = _passwordHasher.HashPassword(user, dto.Password);

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
    public async Task<string> LoginUser(LoginDTO user)
    {
        _logger.LogInformation("Login attempt for user: {Username}", user.Username);

        try
        {
            var getUser = await _repo.GetUserAsync(user.Username);

            if (getUser == null)
            {
                _logger.LogWarning("Login failed: User '{Username}' not found", user.Username);
                return null; 
            }

            _logger.LogInformation("User '{Username}' found. Verifying password...", user.Username);

            var result = _passwordHasher.VerifyHashedPassword(getUser, getUser.Password, user.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login failed: Invalid password for user '{Username}'", user.Username);
                return null;
            }

            _logger.LogInformation("User '{Username}' successfully authenticated", user.Username);
            return CreateToken(getUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while logging in user '{Username}'", user.Username);
            throw; 
        }
    }

    public string CreateToken(Login user)
    {

        var claims = new List<Claim> //claims are info about the user that gets embedded in the token so that the server can verify the user later
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),//determines exactly which the user is the token for 
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token"))); //what will be used to digitally sign the token
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//pairs the secret key with a signing algorithm because the token needs to be signed to verify its authenticity
        
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
