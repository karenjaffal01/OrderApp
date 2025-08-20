using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class LoginService : ILoginService
{
    private readonly ILoginRepository _repo;
    private readonly ILogger<LoginService> _logger;
    private readonly PasswordHasher<Login> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IDbConnection _connection;
    public LoginService(ILoginRepository repo, ILogger<LoginService> logger, IConfiguration configuration,IDbConnection dbConnection)
    {
        _repo = repo;
        _logger = logger;
        _passwordHasher = new PasswordHasher<Login>();
        _configuration = configuration;
        _connection = dbConnection;
    }

    public async Task<Response<object>> RegisterAsync(LoginRequestDTO dto)
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
    public async Task<TokenResponseDTO?> LoginUser(LoginRequestDTO request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        var user = await _repo.GetUserAsync(request.Username);
        if (user == null)
        {
            _logger.LogWarning("Login failed: User '{Username}' not found", request.Username);
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login failed: Invalid password for user '{Username}'", request.Username);
            return null;
        }

        var accessToken = CreateToken(user);
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user); 

        return new TokenResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<Login?> ValidateRefreshTokenAsync(int userId, string refreshToken)
    {
        const string sql = @"
        SELECT ""Id"", ""Username"", ""Password"", ""Role"",
               ""RefreshToken"", ""RefreshTokenExpiryTime"",
               ""IsActive"", ""IsDeleted"", ""CreatedDate""
        FROM public.""Login""
        WHERE ""Id"" = @Id AND ""IsDeleted"" = false;";

        var user = await _connection.QuerySingleOrDefaultAsync<Login>(sql, new { Id = userId });

        if (user == null) return null;
        if (string.IsNullOrEmpty(user.RefreshToken)) return null;
        if (!string.Equals(user.RefreshToken, refreshToken, StringComparison.Ordinal)) return null;
        if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow) return null;

        return user;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private async Task<string> GenerateAndSaveRefreshTokenAsync(Login user, IDbTransaction? transaction = null)
    {
        var refreshToken = GenerateRefreshToken();
        var expiry = DateTime.UtcNow.AddDays(7);

        const string sql = @"
        UPDATE public.""Login""
        SET ""RefreshToken"" = @RefreshToken,
            ""RefreshTokenExpiryTime"" = @Expiry
        WHERE ""Id"" = @Id;";

        await _connection.ExecuteAsync(sql, new
        {
            RefreshToken = refreshToken,
            Expiry = expiry,
            Id = user.Id
        }, transaction);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = expiry;

        return refreshToken;
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

    public async Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request)
    {        
        var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
        if (user is null) return null;
        var response = new TokenResponseDTO
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user) 
        };
        return response;
    }

}
