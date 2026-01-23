using Microsoft.AspNetCore.Mvc;
using Moq;
using Quickaid.Controllers;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace QuickaidBackendTests.ControllerTests
{
    [TestClass]
    public class UsersControllerTests
    {
        private Mock<IUserService> _serviceMock = null!;
        private UsersController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IUserService>();
            _controller = new UsersController(_serviceMock.Object);

            // Fake admin claim
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "1"),
                new Claim(ClaimTypes.Role, "admin")
            ], "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminUser }
            };
        }

        [TestMethod]
        // Test GET /api/users jako admin
        public async Task GetAll_Admin_ReturnsOkWithUsers()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(
            [
                new UserDto { Id = 1, Username = "a" },
                new UserDto { Id = 2, Username = "b" }
            ]);

            var result = await _controller.GetAll();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var users = ok!.Value as IEnumerable<UserDto>;
            Assert.AreEqual(2, users!.Count());
        }

        [TestMethod]
        // Test GET /api/users/{id} dla nieistniejącego użytkownika
        public async Task GetById_UserNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((UserDto?)null);

            var result = await _controller.GetById(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        // Test GET /api/users/{id} jako zwykły użytkownik - dostęp do własnych danych
        public async Task GetById_UserRegular_ReturnsOk()
        {
            var userDto = new UserDto { Id = 5, Username = "user5" };
            _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(userDto);

            // Fake user claim - właściciel
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
                [
            new Claim("id", "5"),
            new Claim(ClaimTypes.Role, "user")
                ], "mock"));

            var result = await _controller.GetById(5);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var ok = result as OkObjectResult;
            var value = ok!.Value as UserDto;
            Assert.AreEqual(5, value!.Id);
            Assert.AreEqual("user5", value.Username);
        }
    }
}
