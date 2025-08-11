using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Entities;

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
            return Ok(response);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Login dto)
        {
            var response = await _service.GetUserAsync(dto.Username, dto.Password);
            return Ok(response);
        }
    }

}
