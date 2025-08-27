using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stock.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create/{itemId}")]
        public async Task<IActionResult> CreateStock(int itemId)
        {
            var response = await _stockService.CreateStockAsync(itemId);

            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateQuantity/{stockId}/{quantity}")]
        public async Task<IActionResult> UpdateStockQuantity(int stockId, int quantity)
        {
            var response = await _stockService.UpdateStockQuantityAsync(stockId, quantity);

            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{stockId}")]
        public async Task<IActionResult> DeleteStock(int stockId)
        {
            var response = await _stockService.DeleteStockAsync(stockId);

            if (response.Code == Response<object>.ErrorCode.Success)
                return Ok(response);

            return NotFound(response);
        }

        [Authorize]
        [HttpGet("quantity/{itemId}")]
        public async Task<IActionResult> GetStockQuantity(int itemId)
        {
            var response = await _stockService.GetStockQuantityAsync(itemId);

            if (response.Code == Response<int>.ErrorCode.Success)
                return Ok(response);

            return NotFound(response);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllStocks()
        {
            var response = await _stockService.GetAllStocksAsync();

            if (response.Code == Response<IEnumerable<dynamic>>.ErrorCode.Success)
                return Ok(response);

            return NotFound(response);
        }
    }
}
