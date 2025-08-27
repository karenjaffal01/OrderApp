using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Controller;
using OrderManagement.Business;
using OrderManagement.Business.Interfaces;
using OrderManagement.Domain;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.DTO;
using OrderManagement.Domain.Requests;
using OrderManagement.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
namespace Orders.Test
{
    public class OrderControllerTests
    {

        [Fact]
        public async Task getAllOrders_ReturnsOkAsync()
        {
            //Arrange
            var _fakeOrderService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeOrderService);
            var fakeOrders = new List<OrderDTO>
            {
                new OrderDTO { Id = 1, CustomerName = "Test Order" }
            };
            var successResponse = new Response<List<OrderDTO>>
            {
                Code = Response<List<OrderDTO>>.ErrorCode.Success,
                Data = fakeOrders
            };
            A.CallTo(() => _fakeOrderService.GetAllOrdersAsync())
             .Returns(Task.FromResult(successResponse)); //simulate an async Task return 
            //Act   
            var result = await _controller.GetAllOrders();
            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result); //check if it actually returned Ok if it returned something else the test would fail
            var returnedResponse = Assert.IsType<Response<List<OrderDTO>>>(okResult.Value);//this is the body and we are checking if it actually is of response.. type
            Assert.Equal(Response<List<OrderDTO>>.ErrorCode.Success, returnedResponse.Code);
            Assert.Equal(fakeOrders.Count, returnedResponse.Data.Count);
        }
        [Fact]
        public async Task getAllOrders_ReturnBadRequestAsync()
        {
            var _fakeOrderService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeOrderService);
            var errorResponse = new Response<List<OrderDTO>>
            {
                Code = Response<List<OrderDTO>>.ErrorCode.Error,
                Message = "An error occurred"
            };
            A.CallTo(() => _fakeOrderService.GetAllOrdersAsync()).Returns(Task.FromResult(errorResponse));
            var result = await _controller.GetAllOrders();
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<List<OrderDTO>>>(badResult.Value);
            Assert.Equal(Response<List<OrderDTO>>.ErrorCode.Error, returnedResponse.Code);
        }
        [Fact]
        public async Task createOrder_ReturnsOk()
        {
            var _fakeOrderService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeOrderService);

            var request = new CreateOrderRequest
            {
                CustomerName = "karen",
                CreatedBy = "System"
            };

            var successResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeOrderService.CreateOrderAsync(A<CreateOrderDTO>.Ignored))
             .Returns(Task.FromResult(successResponse));
            var result = await _controller.CreateOrder(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);

            A.CallTo(() => _fakeOrderService.CreateOrderAsync(
                A<CreateOrderDTO>.That.Matches(dto => dto.CustomerName == "karen")))
             .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task createOrder_ReturnsBadRequest()
        {
            var _fakeOrderService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeOrderService);
            var request = new CreateOrderRequest
            {
                CustomerName = "karen",
                CreatedBy = "System"
            };

            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };
            A.CallTo(() => _fakeOrderService.CreateOrderAsync(A<CreateOrderDTO>.Ignored))
            .Returns(Task.FromResult(failureResponse));
            var result = await _controller.CreateOrder(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequestResult.Value);

            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }
        [Fact]
        public async Task UpdateOrder_ReturnsOk()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var request = new UpdateOrderRequest
            {
                Id = 1,
                CustomerName = "Alice",
                UpdatedBy = "Admin"
            };

            var successResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeService.UpdateOrderAsync(A<updateOrderDTO>.Ignored))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.UpdateOrder(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);
        }

        [Fact]
        public async Task UpdateOrder_ReturnsBadRequest()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var request = new UpdateOrderRequest
            {
                Id = 1,
                CustomerName = "Alice",
                UpdatedBy = "Admin"
            };

            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeService.UpdateOrderAsync(A<updateOrderDTO>.Ignored))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.UpdateOrder(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequest.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }
        [Fact]
        public async Task DeleteOrders_ReturnsOk()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var id = 1;
            var successResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeService.DeleteOrdersAsync(id))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.DeleteOrders(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);
        }

        [Fact]
        public async Task DeleteOrders_ReturnsBadRequest()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var id = 1;
            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeService.DeleteOrdersAsync(id))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.DeleteOrders(id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequest.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }
        [Fact]
        public async Task GetOrderById_ReturnsOk()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var id = 1;
            var fakeOrder = new OrderDTO { Id = id, CustomerName = "Test Order" };
            var successResponse = new Response<OrderDTO>
            {
                Code = Response<OrderDTO>.ErrorCode.Success,
                Data = fakeOrder
            };

            A.CallTo(() => _fakeService.GetOrderByIdAsync(id))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.GetOrderById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<OrderDTO>>(okResult.Value);
            Assert.Equal(Response<OrderDTO>.ErrorCode.Success, returnedResponse.Code);
            Assert.Equal(fakeOrder.Id, returnedResponse.Data.Id);
        }
        [Fact]
        public async Task GetOrderById_ReturnsBadRequest()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var id = 1;
            var failureResponse = new Response<OrderDTO>
            {
                Code = Response<OrderDTO>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeService.GetOrderByIdAsync(id))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.GetOrderById(id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<OrderDTO>>(badRequest.Value);
            Assert.Equal(Response<OrderDTO>.ErrorCode.Error, returnedResponse.Code);
        }
        [Fact]
        public async Task PlaceOrderWithItems_ReturnsOk()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var dto = new PlaceOrderWithItemsDTO
            {
                Order = new CreateOrderDTO { CustomerName = "karen", CreatedBy = "system" },
                OrderItems = new List<CreateOrderItemDTO> { new CreateOrderItemDTO { ProductName = "Item1" } }
            };

            var successResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeService.PlaceOrderWithItemsAsync(dto.Order, dto.OrderItems))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.PlaceOrderWithItems(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);
        }

        [Fact]
        public async Task PlaceOrderWithItems_ReturnsBadRequest()
        {
            var _fakeService = A.Fake<IOrderService>();
            var _controller = new OrdersController(_fakeService);
            var dto = new PlaceOrderWithItemsDTO
            {
                Order = new CreateOrderDTO { CustomerName = "Alice", CreatedBy = "Admin" },
                OrderItems = new List<CreateOrderItemDTO> { new CreateOrderItemDTO { ProductName = "Item1" } }
            };

            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeService.PlaceOrderWithItemsAsync(dto.Order, dto.OrderItems))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.PlaceOrderWithItems(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequest.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }
    }
}
