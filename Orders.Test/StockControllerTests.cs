using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using Stock.API.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Test
{
    public class StockControllerTests
    {
        [Fact]
        public async Task CreateStock_ReturnsOk()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            int itemId = 1;
            var response = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Message = "Stock created successfully",
                Data = new { ItemId = itemId }
            };

            A.CallTo(() => _fakeStockService.CreateStockAsync(itemId)).Returns(Task.FromResult(response));

            var result = await _controller.CreateStock(itemId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);
        }

        [Fact]
        public async Task CreateStock_ReturnsBadRequest()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            int itemId = 1;
            var response = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Message = "Failed to create stock",
                Data = null
            };

            A.CallTo(() => _fakeStockService.CreateStockAsync(itemId)).Returns(Task.FromResult(response));

            var result = await _controller.CreateStock(itemId);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }
        [Fact]
        public async Task UpdateStockQuantity_ReturnsOk()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            int stockId = 1, quantity = 10;
            var response = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Message = "Stock quantity updated successfully",
                Data = new { StockId = stockId, Quantity = quantity }
            };

            A.CallTo(() => _fakeStockService.UpdateStockQuantityAsync(stockId, quantity))
             .Returns(Task.FromResult(response));

            var result = await _controller.UpdateStockQuantity(stockId, quantity);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);
        }

        [Fact]
        public async Task UpdateStockQuantity_ReturnsBadRequest()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            int stockId = 1, quantity = 10;
            var response = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Message = "Failed to update stock quantity",
                Data = null
            };

            A.CallTo(() => _fakeStockService.UpdateStockQuantityAsync(stockId, quantity))
             .Returns(Task.FromResult(response));

            var result = await _controller.UpdateStockQuantity(stockId, quantity);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }
        [Fact]
        public async Task DeleteStock_ReturnsOk()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            int stockId = 1;
            var response = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Message = "Stock deleted successfully",
                Data = null
            };

            A.CallTo(() => _fakeStockService.DeleteStockAsync(stockId))
             .Returns(Task.FromResult(response));

            var result = await _controller.DeleteStock(stockId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);
        }

        [Fact]
        public async Task DeleteStock_ReturnsBadRequest()
        {
            var fakeStockService = A.Fake<IStockService>();
            var controller = new StockController(fakeStockService);
            int stockId = 1;

            var errorResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Message = "Failed to delete stock",
                Data = null
            };
            A.CallTo(() => fakeStockService.DeleteStockAsync(stockId))
             .Returns(Task.FromResult(errorResponse));

            var result = await controller.DeleteStock(stockId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(notFoundResult.Value);

            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
            Assert.Equal("Failed to delete stock", returnedResponse.Message);
            Assert.Null(returnedResponse.Data);
        }
        [Fact]
        public async Task GetStockQuantity_ReturnsOk()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            int itemId = 1;
            var response = new Response<int>
            {
                Code = Response<int>.ErrorCode.Success,
                Message = "Stock quantity retrieved successfully",
                Data = 50
            };

            A.CallTo(() => _fakeStockService.GetStockQuantityAsync(itemId))
             .Returns(Task.FromResult(response));

            var result = await _controller.GetStockQuantity(itemId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<int>>(okResult.Value);
            Assert.Equal(Response<int>.ErrorCode.Success, returnedResponse.Code);
            Assert.Equal(50, returnedResponse.Data);
        }

        [Fact]
        public async Task GetStockQuantity_ReturnsNotFound()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            int itemId = 1;
            var response = new Response<int>
            {
                Code = Response<int>.ErrorCode.Error,
                Message = "Stock not found",
                Data = -1
            };

            A.CallTo(() => _fakeStockService.GetStockQuantityAsync(itemId))
             .Returns(Task.FromResult(response));

            var result = await _controller.GetStockQuantity(itemId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllStocks_ReturnsOk()
        {
            var _fakeStockService = A.Fake<IStockService>();
            var _controller = new StockController(_fakeStockService);
            var stockList = new List<dynamic> { new { Id = 1, Quantity = 50 } };
            var response = new Response<IEnumerable<dynamic>>
            {
                Code = Response<IEnumerable<dynamic>>.ErrorCode.Success,
                Message = "Stocks retrieved successfully",
                Data = stockList
            };

            A.CallTo(() => _fakeStockService.GetAllStocksAsync())
             .Returns(Task.FromResult(response));

            var result = await _controller.GetAllStocks();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<IEnumerable<dynamic>>>(okResult.Value);
            Assert.Equal(Response<IEnumerable<dynamic>>.ErrorCode.Success, returnedResponse.Code);
            Assert.Single(returnedResponse.Data);
        }

    }
}
