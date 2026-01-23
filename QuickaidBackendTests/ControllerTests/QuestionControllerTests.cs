using Microsoft.AspNetCore.Mvc;
using Moq;
using Quickaid.Controllers;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;

namespace QuickaidBackendTests.ControllerTests
{
    [TestClass]
    public class QuestionsControllerTests
    {
        private Mock<IQuestionService> _serviceMock = null!;
        private QuestionsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IQuestionService>();
            _controller = new QuestionsController(_serviceMock.Object);
        }

        [TestMethod]
        // Test GET /api/questions do pobierania wszystkich pytań
        public async Task GetAll_ReturnsOkWithQuestions()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(
            [
                new QuestionDto { Id = 1, QuestionText = "Q1", Answers = [] },
                new QuestionDto { Id = 2, QuestionText = "Q2", Answers = [] }
            ]);

            var result = await _controller.GetAll();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            var questions = ok!.Value as IEnumerable<QuestionDto>;
            Assert.AreEqual(2, questions!.Count());
        }

        [TestMethod]
        // Test GET /api/questions/{id} dla istniejącego pytania
        public async Task GetById_QuestionExists_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new QuestionDto { Id = 1, QuestionText = "Q1", Answers = [] });

            var result = await _controller.GetById(1);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test GET /api/questions/{id} dla nieistniejącego pytania
        public async Task GetById_QuestionNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((QuestionDto?)null);

            var result = await _controller.GetById(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test POST /api/questions do dodawania poprawnego pytania
        public async Task Add_ValidQuestion_ReturnsCreatedAtAction()
        {
            var dto = new QuestionDto { QuestionText = "Q New", Answers = [] };

            _serviceMock.Setup(s => s.AddAsync(dto))
                .ReturnsAsync(new QuestionDto { Id = 10, QuestionText = dto.QuestionText, Answers = dto.Answers });

            var result = await _controller.Add(dto);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var created = (result as CreatedAtActionResult)!.Value as QuestionDto;
            Assert.AreEqual(10, created!.Id);
        }

        [TestMethod]
        // Test PUT /api/questions/{id} dla istniejącego pytania
        public async Task Update_QuestionExists_ReturnsOk()
        {
            var dto = new QuestionDto { QuestionText = "Updated", Answers = [] };

            _serviceMock.Setup(s => s.UpdateAsync(1, dto))
                .ReturnsAsync(new QuestionDto { Id = 1, QuestionText = dto.QuestionText, Answers = dto.Answers });

            var result = await _controller.Update(1, dto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test PUT /api/questions/{id} dla nieistniejącego pytania
        public async Task Update_QuestionNotExists_ReturnsNotFound()
        {
            var dto = new QuestionDto { QuestionText = "Updated", Answers = [] };

            _serviceMock.Setup(s => s.UpdateAsync(99, dto))
                .ReturnsAsync((QuestionDto?)null);

            var result = await _controller.Update(99, dto);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test DELETE /api/questions/{questionId}/quiz/{quizId} dla istniejącego pytania
        public async Task Delete_QuestionExists_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1, 1))
                .ReturnsAsync(true);

            var result = await _controller.Delete(1, 1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        // Test DELETE /api/questions/{questionId}/quiz/{quizId} dla nieistniejącego pytania
        public async Task Delete_QuestionNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(99, 1))
                .ReturnsAsync(false);

            var result = await _controller.Delete(99, 1);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        // Test POST /api/questions/quiz/{quizId} do dodawania pytania do quizu
        public async Task AddQuestionToQuiz_Valid_ReturnsCreated()
        {
            var dto = new QuestionDto { QuestionText = "Q New", Answers = [] };

            _serviceMock.Setup(s => s.AddToQuizAsync(1, dto))
                .ReturnsAsync(new QuestionDto { Id = 10, QuestionText = dto.QuestionText, Answers = dto.Answers });

            var result = await _controller.AddQuestionToQuiz(1, dto);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        // Test POST /api/questions dla niepoprawnego modelu
        public async Task Add_InvalidQuestion_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("QuestionText", "Required");
            var dto = new QuestionDto { QuestionText = "", Answers = [] };

            var result = await _controller.Add(dto);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        // Test PUT /api/questions/{id} dla niepoprawnego modelu
        public async Task Update_InvalidQuestion_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("QuestionText", "Required");
            var dto = new QuestionDto { QuestionText = "", Answers = [] };

            var result = await _controller.Update(1, dto);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
