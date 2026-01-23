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
    public class ResultsControllerTests
    {
        private Mock<IResultService> _serviceMock = null!;
        private ResultsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IResultService>();
            _controller = new ResultsController(_serviceMock.Object);

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
        // Test GET /api/results jako admin
        public async Task GetAll_Admin_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new[]
            {
                new ResultDto { Id = 1 },
                new ResultDto { Id = 2 }
            });

            var result = await _controller.GetAll();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            var results = ok!.Value as IEnumerable<ResultDto>;
            Assert.AreEqual(2, results!.Count());
        }

        [TestMethod]
        // Test GET /api/results/{id} dla właściciela, innego użytkownika i admina
        public async Task GetById_User_ReturnsOkOrForbid()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(2))
                .ReturnsAsync(new ResultDto { Id = 2, UserId = 2 });

            // Fake user claim - właściciel wyniku
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "2"),
                new Claim(ClaimTypes.Role, "user")
            ], "mock"));
            var own = await _controller.GetById(2);
            Assert.IsInstanceOfType(own, typeof(OkObjectResult));

            // Fake user claim - inny użytkownik
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "1"),
                new Claim(ClaimTypes.Role, "user")
            ], "mock"));
            var forbid = await _controller.GetById(2);
            Assert.IsInstanceOfType(forbid, typeof(ForbidResult));

            // Fake admin claim
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "1"),
                new Claim(ClaimTypes.Role, "admin")
            ], "mock"));
            var adminResult = await _controller.GetById(2);
            Assert.IsInstanceOfType(adminResult, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test POST /api/results - zwykły user powinien mieć ustawiony swój userId
        public async Task Add_User_SetsUserId()
        {
            var dto = new ResultDto
            {
                QuizId = 1,
                Score = 100,
                CompletedAt = DateTime.UtcNow
            };

            _serviceMock.Setup(s => s.AddAsync(It.IsAny<ResultDto>()))
                .ReturnsAsync((ResultDto r) => r);

            // Fake user claim
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "5"),
                new Claim(ClaimTypes.Role, "user")
            ], "mock"));

            _controller.ModelState.Clear();

            var result = await _controller.Add(dto);
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var created = ok!.Value as ResultDto;
            Assert.IsNotNull(created);
            Assert.AreEqual(5, created!.UserId);
        }

        [TestMethod]
        // Test DELETE /api/results/{id} - właściciel, inny użytkownik i admin
        public async Task Delete_User_ReturnsNoContentOrForbid()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ResultDto { Id = 1, UserId = 1 });
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Fake user claim - właściciel wyniku
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "1"),
                new Claim(ClaimTypes.Role, "user")
            ], "mock"));
            var result = await _controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            // Fake user claim - inny użytkownik
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "2"),
                new Claim(ClaimTypes.Role, "user")
            ], "mock"));
            result = await _controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(ForbidResult));

            // Fake admin claim
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "99"),
                new Claim(ClaimTypes.Role, "admin")
            ], "mock"));
            result = await _controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
