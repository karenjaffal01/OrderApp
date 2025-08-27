using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Controller;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Test
{
    public class OrderItemControllerTests
    {
        [Fact]
        public async Task AddOrderItem_ReturnsOk()
        {
            var _fakeService = A.Fake<IOrderItemService>();
            var _controller = new OrderItemController(_fakeService);
            var dto = new CreateOrderItemDTO { ProductName = "Item1", Quantity = 2 };

            var successResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeService.AddOrderItemAsync(A<CreateOrderItemDTO>.Ignored))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.AddOrderItem(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);

            A.CallTo(() => _fakeService.AddOrderItemAsync(
                A<CreateOrderItemDTO>.That.Matches(x => x.ProductName == "Item1" && x.Quantity == 2)))
             .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddOrderItem_ReturnsBadRequest()
        {
            var _fakeService = A.Fake<IOrderItemService>();
            var _controller = new OrderItemController(_fakeService);
            var dto = new CreateOrderItemDTO { ProductName = "Item1", Quantity = 2 };

            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeService.AddOrderItemAsync(A<CreateOrderItemDTO>.Ignored))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.AddOrderItem(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequest.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }

        [Fact]
        public async Task UpdateOrderItem_ReturnsOk()
        {
            var _fakeService = A.Fake<IOrderItemService>();
            var _controller = new OrderItemController(_fakeService);
            var dto = new UpdateOrderItemDTO { ProductName = "Item1", Quantity = 3 };
            int id = 1;

            var successResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeService.UpdateOrderItemAsync(A<UpdateOrderItemDTO>.Ignored))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.UpdateOrderItem(id, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);

            A.CallTo(() => _fakeService.UpdateOrderItemAsync(
                A<UpdateOrderItemDTO>.That.Matches(x => x.Id == id && x.ProductName == "Item1" && x.Quantity == 3)))
             .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateOrderItem_ReturnsBadRequest()
        {
            var _fakeService = A.Fake<IOrderItemService>();
            var _controller = new OrderItemController(_fakeService);
            var dto = new UpdateOrderItemDTO { ProductName = "Item1", Quantity = 3 };
            int id = 1;

            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeService.UpdateOrderItemAsync(A<UpdateOrderItemDTO>.Ignored))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.UpdateOrderItem(id, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequest.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }

        [Fact]
        public async Task DeleteOrderItem_ReturnsOk()
        {
            var _fakeService = A.Fake<IOrderItemService>();
            var _controller = new OrderItemController(_fakeService);
            int id = 1;

            var successResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeService.DeleteOrderItemAsync(id))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.DeleteOrderItem(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);

            A.CallTo(() => _fakeService.DeleteOrderItemAsync(id))
             .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DeleteOrderItem_ReturnsBadRequest()
        {
            var _fakeService = A.Fake<IOrderItemService>();
            var _controller = new OrderItemController(_fakeService);
            int id = 1;

            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeService.DeleteOrderItemAsync(id))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.DeleteOrderItem(id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequest.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }
    }
}
