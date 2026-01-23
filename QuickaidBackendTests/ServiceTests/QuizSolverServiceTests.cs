using Moq;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Services;
using Quickaid.Services.Interfaces;
using QuickaidBackendTests.TestHelpers;

namespace QuickaidBackendTests.ServiceTests
{
    [TestClass]
    public class QuizSolverServiceTests
    {
        private AppDbContext _db = null!;
        private Mock<IResultService> _resultServiceMock = null!;
        private QuizSolverService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();
            _resultServiceMock = new Mock<IResultService>();
            _service = new QuizSolverService(_db, _resultServiceMock.Object);
        }

        [TestMethod]
        // Test CalculateScoreAsync - oblicza poprawny wynik na podstawie odpowiedzi użytkownika
        public async Task CalculateScoreAsync_ReturnsCorrectScore()
        {
            DbHelper.SeedQuizzes(_db);

            var userAnswers = new Dictionary<int, int>
            {
                { 10, 1 }, // poprawna odpowiedź
                { 20, 3 }, // poprawna odpowiedź
                { 30, 6 }  // błędna odpowiedź
            };

            var score = await _service.CalculateScoreAsync(1, userAnswers);
            Assert.AreEqual(2, score);
        }

        [TestMethod]
        // Test SubmitQuizAsync - dodaje wynik quizu z poprawnym wynikiem
        public async Task SubmitQuizAsync_AddsResultWithCorrectScore()
        {
            DbHelper.SeedQuizzes(_db);

            var userAnswers = new Dictionary<int, int> { { 10, 1 } };
            _resultServiceMock.Setup(r => r.AddAsync(It.IsAny<ResultDto>()))
                .ReturnsAsync((ResultDto r) => r);

            var result = await _service.SubmitQuizAsync(5, 1, userAnswers);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.UserId);
            Assert.AreEqual(1, result.QuizId);
            Assert.AreEqual(1, result.Score);
        }

        [TestMethod]
        // Test GetLastResultAsync - zwraca ostatni wynik dla użytkownika lub null, jeśli brak wyników
        public async Task GetLastResultAsync_ReturnsMostRecentResultOrNull()
        {
            DbHelper.SeedResults(_db);

            var lastResult = await _service.GetLastResultAsync(1, 1);
            Assert.IsNotNull(lastResult);
            Assert.AreEqual(2, lastResult!.Id);
            Assert.AreEqual(90, lastResult.Score);

            var missing = await _service.GetLastResultAsync(99, 1);
            Assert.IsNull(missing);
        }
    }
}
