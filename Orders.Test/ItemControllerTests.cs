using FakeItEasy;
using Item.API.Controller;
using Microsoft.AspNetCore.Mvc;
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
    public class ItemControllerTests
    {
        [Fact]
        public async Task CreateItem_ReturnsOk_WhenSuccess()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);
            var dto = new CreateItemDTO { ItemName = "TestItem" };

            var response = new Response<object>
            {
                Code = Response<object>.ErrorCode.Success,
                Message = "Item created",
                Data = null
            };
            A.CallTo(() => fakeService.CreateItemAsync(dto))
                .Returns(Task.FromResult(response));

            var result = await controller.CreateItem(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);
            Assert.Equal("Item created", returnedResponse.Message);
        }
        [Fact]
        public async Task CreateItem_ReturnsBadRequest()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);
            var dto = new CreateItemDTO { ItemName = "TestItem" };

            var response = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Message = "Failed to create item",
                Data = null
            };

            A.CallTo(() => fakeService.CreateItemAsync(dto))
                .Returns(Task.FromResult(response));

            var result = await controller.CreateItem(dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequestResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
            Assert.Equal("Failed to create item", returnedResponse.Message);
        }

        [Fact]
        public async Task DeleteItem_ReturnsOk()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);
            int id = 1;

            A.CallTo(() => fakeService.DeleteItem(id))
                .Returns(Task.FromResult((0, "Deleted successfully")));

            var result = await controller.DeleteItem(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = okResult.Value;

            var errorCodeProp = returned.GetType().GetProperty("errorCode")?.GetValue(returned, null);
            var messageProp = returned.GetType().GetProperty("message")?.GetValue(returned, null);

            Assert.Equal(0, errorCodeProp);
            Assert.Equal("Deleted successfully", messageProp);
        }

        [Fact]
        public async Task DeleteItem_ReturnsBadRequest()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);
            int id = 1;

            A.CallTo(() => fakeService.DeleteItem(id))
                .Returns(Task.FromResult((1, "Failed to delete")));

            var result = await controller.DeleteItem(id);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returned = badRequestResult.Value;

            var errorCodeProp = returned.GetType().GetProperty("errorCode")?.GetValue(returned, null);
            var messageProp = returned.GetType().GetProperty("message")?.GetValue(returned, null);

            Assert.Equal(1, errorCodeProp);
            Assert.Equal("Failed to delete", messageProp);
        }

        [Fact]
        public async Task UpdateItem_ReturnsOk()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);
            var dto = new UpdateItemDTO { ItemId = 1, ItemName = "UpdatedItem" };

            A.CallTo(() => fakeService.UpdateItem(dto))
                .Returns(Task.FromResult((0, "Updated successfully")));

            var result = await controller.UpdateItem(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = okResult.Value;

            var errorCodeProp = returned.GetType().GetProperty("errorCode")?.GetValue(returned, null);
            var messageProp = returned.GetType().GetProperty("message")?.GetValue(returned, null);

            Assert.Equal(0, errorCodeProp);
            Assert.Equal("Updated successfully", messageProp);
        }
        [Fact]
        public async Task UpdateItem_ReturnsBadRequest()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);
            var dto = new UpdateItemDTO { ItemId = 1, ItemName = "UpdatedItem" };

            A.CallTo(() => fakeService.UpdateItem(dto))
                .Returns(Task.FromResult((1, "Failed to update")));

            var result = await controller.UpdateItem(dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returned = badRequestResult.Value;

            var errorCodeProp = returned.GetType().GetProperty("errorCode")?.GetValue(returned, null);
            var messageProp = returned.GetType().GetProperty("message")?.GetValue(returned, null);

            Assert.Equal(1, errorCodeProp);
            Assert.Equal("Failed to update", messageProp);
        }


        [Fact]
        public async Task GetItems_ReturnsOk()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);

            var items = new List<ItemDTO> { new ItemDTO { ItemId = 1, ItemName = "Item1" } };

            A.CallTo(() => fakeService.GetItems())
                .Returns(Task.FromResult((IEnumerable<ItemDTO>)items));

            var result = await controller.GetItems();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItems = Assert.IsAssignableFrom<IEnumerable<ItemDTO>>(okResult.Value);
            Assert.Single(returnedItems);
        }

        [Fact]
        public async Task GetItems_ReturnsNotFound()
        {
            var fakeService = A.Fake<IItemService>();
            var controller = new ItemController(fakeService);

            A.CallTo(() => fakeService.GetItems())
                .Returns(Task.FromResult((IEnumerable<ItemDTO>)null));

            var result = await controller.GetItems();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No items found.", notFoundResult.Value);
        }
    }
}
