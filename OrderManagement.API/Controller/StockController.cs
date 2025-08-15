using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagement.API.Controller
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
            var (errorCode, message) = await _stockService.CreateStock(itemId);

            if (errorCode == 0)
                return Ok(new { message });
            else
                return BadRequest(new { message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateQuantity/{stockId}/{quantity}")]
        public async Task<IActionResult> UpdateStockQuantity(int stockId, int quantity)
        {
            var (errorCode, message) = await _stockService.UpdateStockQuantity(stockId, quantity);

            if (errorCode == 0)
                return Ok(new { message });
            else
                return BadRequest(new { message });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{stockId}")]
        public async Task<IActionResult> DeleteStock(int stockId)
        {
            var (errorCode, message) = await _stockService.DeleteStock(stockId);

            if (errorCode == 0)
                return Ok(new { message });
            else
                return NotFound(new { message });
        }

        [Authorize]
        [HttpGet("quantity/{itemId}")]
        public async Task<IActionResult> GetStockQuantity(int itemId)
        {
            int quantity = await _stockService.GetStockQuantity(itemId);
            if (quantity >= 0)
                return Ok(new { itemId, quantity });
            else
                return NotFound(new { message = "Stock not found" });
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllStocks()
        {
            IEnumerable<dynamic> stocks = await _stockService.GetAllStocks();
            if (stocks != null)
                return Ok(stocks);
            else
                return NotFound(new { message = "No stocks found" });
        }
    }
}
