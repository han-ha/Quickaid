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
    public class QuizzesControllerTests
    {
        private Mock<IQuizService> _quizServiceMock;
        private Mock<IQuizSolverService> _quizSolverMock;
        private Mock<IQuestionService> _questionServiceMock;
        private QuizzesController _controller;

        [TestInitialize]
        public void Setup()
        {
            _quizServiceMock = new Mock<IQuizService>();
            _quizSolverMock = new Mock<IQuizSolverService>();
            _questionServiceMock = new Mock<IQuestionService>();

            _controller = new QuizzesController(
                _quizServiceMock.Object,
                _quizSolverMock.Object,
                _questionServiceMock.Object
            );

            // Fake admin claim
            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new("id", "1"),
                new(ClaimTypes.Role, "admin")
            ], "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        // Test GET /api/quizzes do pobrania wszystkich quizów
        public async Task GetAll_ReturnsOkWithQuizzes()
        {
            _quizServiceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(
                [
                    new QuizDto { Id = 1, Title = "Q1" },
                    new QuizDto { Id = 2, Title = "Q2" }
                ]);

            var result = await _controller.GetAll();
            var ok = result as OkObjectResult;

            Assert.IsNotNull(ok);
            var quizzes = ok!.Value as IEnumerable<QuizDto>;
            Assert.AreEqual(2, quizzes!.Count());
        }

        [TestMethod]
        // Test GET /api/quizzes/{id} dla istniejącego i nieistniejącego quizu
        public async Task GetById_ReturnsOkOrNotFound()
        {
            _quizServiceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new QuizDto { Id = 1 });

            var existing = await _controller.GetById(1);
            Assert.IsInstanceOfType(existing, typeof(OkObjectResult));

            _quizServiceMock.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((QuizDto?)null);

            var missing = await _controller.GetById(99);
            Assert.IsInstanceOfType(missing, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test POST /api/quizzes do dodania quizu jako admin
        public async Task Add_Admin_ReturnsCreated()
        {
            var dto = new QuizDto { Title = "New Quiz" };
            _quizServiceMock.Setup(s => s.AddAsync(dto))
                .ReturnsAsync(new QuizDto { Id = 10, Title = "New Quiz" });

            _controller.ModelState.Clear();

            var result = await _controller.Add(dto);
            var created = result as CreatedAtActionResult;

            Assert.IsNotNull(created);
            var quiz = created!.Value as QuizDto;
            Assert.AreEqual(10, quiz!.Id);
        }

        [TestMethod]
        // Test PUT /api/quizzes/{id} do aktualizacji quizu jako admin dla istniejącego i nieistniejącego quizu
        public async Task Update_Admin_ReturnsOkOrNotFound()
        {
            var dto = new QuizDto { Title = "Updated" };
            _quizServiceMock.Setup(s => s.UpdateAsync(1, dto))
                .ReturnsAsync(new QuizDto { Id = 1, Title = "Updated" });

            var ok = await _controller.Update(1, dto);
            Assert.IsInstanceOfType(ok, typeof(OkObjectResult));

            _quizServiceMock.Setup(s => s.UpdateAsync(99, dto))
                .ReturnsAsync((QuizDto?)null);

            var notFound = await _controller.Update(99, dto);
            Assert.IsInstanceOfType(notFound, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test DELETE /api/quizzes/{id} do usunięcia quizu jako admin dla istniejącego i nieistniejącego quizu
        public async Task Delete_Admin_ReturnsNoContentOrNotFound()
        {
            _quizServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            _quizServiceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var noContent = await _controller.Delete(1);
            Assert.IsInstanceOfType(noContent, typeof(NoContentResult));

            var notFound = await _controller.Delete(99);
            Assert.IsInstanceOfType(notFound, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test POST /api/quizzes/{quizId}/submit do przesłania quizu przez użytkownika
        public async Task SubmitQuiz_Valid_ReturnsOk()
        {
            // Fake user claim
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [
                        new("id", "5"),
                        new(ClaimTypes.Role, "user")
                    ], "mock"))
                }
            };

            var answers = new Dictionary<int, int> { { 1, 2 } };
            var submission = new QuizSubmissionDto { Answers = answers };

            _quizServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new QuizDto { Id = 1 });
            _quizServiceMock.Setup(s => s.GetQuestionsIdsAsync(1)).ReturnsAsync([1]);
            _quizSolverMock.Setup(s => s.SubmitQuizAsync(5, 1, answers))
                .ReturnsAsync(new ResultDto { Id = 100, UserId = 5, Score = 1 });

            var result = await _controller.SubmitQuiz(1, submission);
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var res = ok!.Value as ResultDto;
            Assert.AreEqual(5, res!.UserId);
        }

        [TestMethod]
        // Test GET /api/quizzes/{quizId}/results/me do pobrania ostatniego wyniku użytkownika
        // dla istniejącego i nieistniejącego quizu
        public async Task GetMyLastResult_ReturnsOkOrNotFound()
        {
            // Fake user claim
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [
                        new("id", "5"),
                        new(ClaimTypes.Role, "user")
                    ], "mock"))
                }
            };

            _quizSolverMock.Setup(s => s.GetLastResultAsync(5, 1))
                .ReturnsAsync(new ResultDto { Id = 1, UserId = 5 });

            var ok = await _controller.GetMyLastResult(1);
            Assert.IsInstanceOfType(ok, typeof(OkObjectResult));

            _quizSolverMock.Setup(s => s.GetLastResultAsync(5, 99))
                .ReturnsAsync((ResultDto?)null);

            var notFound = await _controller.GetMyLastResult(99);
            Assert.IsInstanceOfType(notFound, typeof(NotFoundObjectResult));
        }
    }
}
