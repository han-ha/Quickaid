using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Quickaid.Controllers;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickaidApiTests.ControllerTests
{
    [TestClass]
    public class AnswersControllerTests
    {
        private Mock<IAnswerService> _serviceMock;
        private AnswersController _controller;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IAnswerService>();
            _controller = new AnswersController(_serviceMock.Object);

            // Fake admin claim, żeby można było testować metody admin
            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", "1"),
                new Claim(ClaimTypes.Role, "admin")
            ], "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        // Test GET /api/answers dla admina
        public async Task GetAll_ReturnsOkWithAnswers()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(
                [
                    new AnswerDto { Id = 1, AnswerText = "A", IsCorrect = true },
                    new AnswerDto { Id = 2, AnswerText = "B", IsCorrect = false }
                ]);

            var result = await _controller.GetAll();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var answers = ok!.Value as IEnumerable<AnswerDto>;
            Assert.AreEqual(2, answers!.Count());
        }

        [TestMethod]
        // Test GET /api/answers/{id} dla istniejącej odpowiedzi
        public async Task GetById_AnswerExists_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new AnswerDto { Id = 1, AnswerText = "A", IsCorrect = true });

            var result = await _controller.GetById(1);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test GET /api/answers/{id} dla nieistniejącej odpowiedzi
        public async Task GetById_AnswerNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((AnswerDto?)null);

            var result = await _controller.GetById(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test POST /api/answers?questionId=... dla dodania odpowiedzi
        public async Task Add_ValidAnswer_ReturnsCreatedAtAction()
        {
            var dto = new AnswerDto { AnswerText = "New", IsCorrect = true };
            _serviceMock.Setup(s => s.AddAsync(dto, 10))
                .ReturnsAsync(new AnswerDto { Id = 100, AnswerText = dto.AnswerText, IsCorrect = dto.IsCorrect });

            var result = await _controller.Add(dto, 10);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));

            var created = (result as CreatedAtActionResult)!.Value as AnswerDto;
            Assert.AreEqual(100, created!.Id);
        }

        [TestMethod]
        // Test PUT /api/answers/{id} dla istniejącej odpowiedzi
        public async Task Update_AnswerExists_ReturnsOk()
        {
            var dto = new AnswerDto { AnswerText = "Updated", IsCorrect = false };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto))
                .ReturnsAsync(new AnswerDto { Id = 1, AnswerText = dto.AnswerText, IsCorrect = dto.IsCorrect });

            var result = await _controller.Update(1, dto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test PUT /api/answers/{id} dla nieistniejącej odpowiedzi
        public async Task Update_AnswerNotExists_ReturnsNotFound()
        {
            var dto = new AnswerDto { AnswerText = "Updated", IsCorrect = false };
            _serviceMock.Setup(s => s.UpdateAsync(99, dto))
                .ReturnsAsync((AnswerDto?)null);

            var result = await _controller.Update(99, dto);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test DELETE /api/answers/{id} dla istniejącej odpowiedzi
        public async Task Delete_AnswerExists_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        // Test DELETE /api/answers/{id} dla nieistniejącej odpowiedzi
        public async Task Delete_AnswerNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(99))
                .ReturnsAsync(false);

            var result = await _controller.Delete(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
