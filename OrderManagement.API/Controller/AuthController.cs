using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace OrderManagement.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public AuthController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginDTO dto)
        {
            var loginDto = new LoginDTO
            {
                Username = dto.Username,
                Password = dto.Password
            };

            var response = await _loginService.RegisterAsync(loginDto);

            if (response.Code == (int)Response<object>.ErrorCode.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] LoginRequestDTO dto)
        {
            var result = await _loginService.LoginUser(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(result); 
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            var result = await _loginService.RefreshTokensAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            return Ok(result);
        }
    }
}
