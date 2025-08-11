using Microsoft.Extensions.Logging;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Threading.Tasks;

namespace OrderManagement.Business.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _repo;
        private readonly ILogger<LoginService> _logger;

        public LoginService(ILoginRepository repo, ILogger<LoginService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Response<object>> CreateUserAsync(Login dto)
        {
            _logger.LogInformation("Attempting to create user with username: {Username}", dto.Username);

            try
            {
                var result = await _repo.CreateUserAsync(dto);
                if (result.ErrorCode == 0)
                {
                    _logger.LogInformation("User '{Username}' created successfully", dto.Username);
                }
                else
                {
                    _logger.LogWarning("Failed to create user '{Username}'. Reason: {Message}", dto.Username, result.Message);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating user '{Username}'", dto.Username);
                return new Response<object>
                {
                    Message = ex.Message,
                    Data = null,
                    ErrorCode = -1
                };
            }
        }

        public async Task<Response<object>> GetUserAsync(string username, string password)
        {
            _logger.LogInformation("Checking credentials for username: {Username}", username);

            try
            {
                var result = await _repo.GetUserAsync(username, password);
                if (result.ErrorCode == 0)
                {
                    _logger.LogInformation("User '{Username}' found and authenticated", username);
                }
                else
                {
                    _logger.LogWarning("Authentication failed for username '{Username}'. Reason: {Message}", username, result.Message);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while authenticating user '{Username}'", username);
                return new Response<object>
                {
                    Message = ex.Message,
                    Data = null,
                    ErrorCode = -1
                };
            }
        }
    }
}
