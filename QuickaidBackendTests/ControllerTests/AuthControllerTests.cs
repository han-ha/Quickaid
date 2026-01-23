using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Quickaid.Controllers;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using System.Threading.Tasks;

namespace QuickaidBackendTests.ControllerTests
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _serviceMock = null!;
        private AuthController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IAuthService>();
            _controller = new AuthController(_serviceMock.Object);
        }

        [TestMethod]
        // Test POST /api/auth/register dla poprawnej rejestracji
        public async Task Register_Valid_ReturnsOk()
        {
            var dto = new RegisterDto { Username = "u", Email = "e@test.com", Password = "StrongPass1" };

            _serviceMock.Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync(new AuthResultDto { Success = true, Token = "token" });

            var result = await _controller.Register(dto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test POST /api/auth/register dla niepowodzenia serwisu
        public async Task Register_ServiceFails_ReturnsBadRequest()
        {
            var dto = new RegisterDto { Username = "u", Email = "e@test.com", Password = "StrongPass1" };

            _serviceMock.Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync(new AuthResultDto { Success = false, Message = "Error" });

            var result = await _controller.Register(dto);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        // Test POST /api/auth/register dla niepoprawnego modelu
        public async Task Register_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Username", "Required");
            var dto = new RegisterDto();

            var result = await _controller.Register(dto);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        // Test POST /api/auth/login dla poprawnego logowania
        public async Task Login_Valid_ReturnsOk()
        {
            var dto = new LoginDto { Username = "u", Password = "pass" };

            _serviceMock.Setup(s => s.LoginAsync(dto))
                .ReturnsAsync(new AuthResultDto { Success = true, Token = "token" });

            var result = await _controller.Login(dto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test POST /api/auth/login dla niepowodzenia serwisu
        public async Task Login_ServiceFails_ReturnsUnauthorized()
        {
            var dto = new LoginDto { Username = "u", Password = "pass" };

            _serviceMock.Setup(s => s.LoginAsync(dto))
                .ReturnsAsync(new AuthResultDto { Success = false, Message = "Invalid" });

            var result = await _controller.Login(dto);
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        // Test POST /api/auth/login dla niepoprawnego modelu
        public async Task Login_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Username", "Required");
            var dto = new LoginDto();

            var result = await _controller.Login(dto);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
