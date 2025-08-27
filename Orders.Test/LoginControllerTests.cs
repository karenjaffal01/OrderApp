
using FakeItEasy;
using Login.API.Controller;
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
    public class LoginControllerTests
    {
        [Fact]
        public async Task Register_ReturnsOk()
        {
            var _fakeLoginService = A.Fake<ILoginService>();
            var _controller = new AuthController(_fakeLoginService);
            var dto = new LoginRequestDTO { Username = "karen", Password = "1234" };

            var successResponse = new Response<object>
            {
                Code = (int)Response<object>.ErrorCode.Success,
                Data = null
            };

            A.CallTo(() => _fakeLoginService.RegisterAsync(A<LoginRequestDTO>.Ignored))
             .Returns(Task.FromResult(successResponse));

            var result = await _controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(okResult.Value);
            Assert.Equal(Response<object>.ErrorCode.Success, returnedResponse.Code);

            A.CallTo(() => _fakeLoginService.RegisterAsync(
                A<LoginRequestDTO>.That.Matches(x => x.Username == "karen")))
             .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Register_ReturnsBadRequest()
        {
            var _fakeLoginService = A.Fake<ILoginService>();
            var _controller = new AuthController(_fakeLoginService);
            var dto = new LoginRequestDTO { Username = "karen", Password = "1234" };

            var failureResponse = new Response<object>
            {
                Code = Response<object>.ErrorCode.Error,
                Data = null
            };

            A.CallTo(() => _fakeLoginService.RegisterAsync(A<LoginRequestDTO>.Ignored))
             .Returns(Task.FromResult(failureResponse));

            var result = await _controller.Register(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResponse = Assert.IsType<Response<object>>(badRequest.Value);
            Assert.Equal(Response<object>.ErrorCode.Error, returnedResponse.Code);
        }

        [Fact]
        public async Task Login_ReturnsOk()
        {
            var _fakeLoginService = A.Fake<ILoginService>();
            var _controller = new AuthController(_fakeLoginService);
            var dto = new LoginRequestDTO { Username = "karen", Password = "1234" };
            var tokenResponse = new TokenResponseDTO { AccessToken = "abc123", RefreshToken = "refresh123" };

            A.CallTo(() => _fakeLoginService.LoginUser(A<LoginRequestDTO>.Ignored))
             .Returns(Task.FromResult(tokenResponse));

            var result = await _controller.Login(dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedToken = Assert.IsType<TokenResponseDTO>(okResult.Value);
            Assert.Equal("abc123", returnedToken.AccessToken);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized()
        {
            var _fakeLoginService = A.Fake<ILoginService>();
            var _controller = new AuthController(_fakeLoginService);
            var dto = new LoginRequestDTO { Username = "karen", Password = "wrongpass" };

            A.CallTo(() => _fakeLoginService.LoginUser(A<LoginRequestDTO>.Ignored))
             .Returns(Task.FromResult<TokenResponseDTO>(null));

            var result = await _controller.Login(dto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task RefreshToken_ReturnsOk()
        {
            var _fakeLoginService = A.Fake<ILoginService>();
            var _controller = new AuthController(_fakeLoginService);
            var request = new RefreshTokenRequestDTO { UserId = 1, RefreshToken = "refresh123" };
            var tokenResponse = new TokenResponseDTO { AccessToken = "newToken", RefreshToken = "newRefresh" };

            A.CallTo(() => _fakeLoginService.RefreshTokensAsync(A<RefreshTokenRequestDTO>.Ignored))
             .Returns(Task.FromResult(tokenResponse));

            var result = await _controller.RefreshToken(request);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedToken = Assert.IsType<TokenResponseDTO>(okResult.Value);
            Assert.Equal("newToken", returnedToken.AccessToken);
        }

        [Fact]
        public async Task RefreshToken_ReturnsUnauthorized_WhenServiceReturnsNull()
        {
            var _fakeLoginService = A.Fake<ILoginService>();
            var _controller = new AuthController(_fakeLoginService);
            var request = new RefreshTokenRequestDTO { UserId = 1, RefreshToken = "wrongRefresh" };

            A.CallTo(() => _fakeLoginService.RefreshTokensAsync(A<RefreshTokenRequestDTO>.Ignored))
             .Returns(Task.FromResult<TokenResponseDTO>(null));

            var result = await _controller.RefreshToken(request);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
    }
}
