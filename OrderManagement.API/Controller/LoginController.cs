using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Common; 

namespace OrderManagement.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _service;

        public LoginController(ILoginService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] Login dto)
        {
            var response = await _service.CreateUserAsync(dto);
            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Login dto)
        {
            var response = await _service.GetUserAsync(dto.Username, dto.Password);
            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);
            else
                return Unauthorized(response); 
        }
    }
}
