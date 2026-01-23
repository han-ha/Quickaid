using Moq;
using Quickaid.Data;
using Quickaid.Mapping.Interfaces;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services;
using Quickaid.Services.Interfaces;
using QuickaidBackendTests.TestHelpers;
namespace QuickaidBackendTests.ServiceTests
{
    [TestClass]
    public class QuizServiceTests
    {
        private AppDbContext _db = null!;
        private Mock<IQuizMapper> _mapperMock = null!;
        private Mock<IQuestionService> _questionServiceMock = null!;
        private QuizService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();
            _mapperMock = new Mock<IQuizMapper>();
            _questionServiceMock = new Mock<IQuestionService>();

            _service = new QuizService(_db, _mapperMock.Object, _questionServiceMock.Object);
        }

        [TestMethod]
        // Test GetAllAsync - zwraca wszystkie quizy z bazy
        public async Task GetAllAsync_ReturnsAllQuizzes()
        {
            DbHelper.SeedQuizzes(_db);

            _mapperMock.Setup(m => m.ToDto(It.IsAny<Quiz>()))
                .Returns<Quiz>(q => new QuizDto { Id = q.Id, Title = q.Title });

            var result = await _service.GetAllAsync();
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(q => q.Title == "Quiz 1"));
            Assert.IsTrue(result.Any(q => q.Title == "Quiz 2"));
        }

        [TestMethod]
        // Test GetByIdAsync - zwraca quiz dla istniejącego Id lub null dla nieistniejącego
        public async Task GetByIdAsync_ReturnsQuizOrNull()
        {
            DbHelper.SeedQuizzes(_db);

            _questionServiceMock.Setup(q => q.GetByQuizIdAsync(1))
                .ReturnsAsync([]);

            _mapperMock.Setup(m => m.ToDto(It.IsAny<Quiz>()))
                .Returns<Quiz>(q => new QuizDto { Id = q.Id, Title = q.Title });

            var found = await _service.GetByIdAsync(1);
            Assert.IsNotNull(found);
            Assert.AreEqual(1, found!.Id);

            var missing = await _service.GetByIdAsync(99);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test AddAsync - dodaje nowy quiz i zwraca DTO
        public async Task AddAsync_AddsQuizSuccessfully()
        {
            var dto = new QuizDto { Title = "New Quiz" };
            var entity = new Quiz { Id = 10, Title = dto.Title };

            _mapperMock.Setup(m => m.ToEntity(dto)).Returns(entity);
            _mapperMock.Setup(m => m.ToDto(entity)).Returns(dto);

            var result = await _service.AddAsync(dto);
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Title, result.Title);

            var inDb = await _db.Quizzes.FindAsync(entity.Id);
            Assert.IsNotNull(inDb);
        }

        [TestMethod]
        // Test UpdateAsync - aktualizuje istniejący quiz lub zwraca null dla nieistniejącego
        public async Task UpdateAsync_UpdatesQuizOrReturnsNull()
        {
            DbHelper.SeedQuizzes(_db);

            var dto = new QuizDto { Title = "Updated", Description = "New description" };

            _mapperMock.Setup(m => m.ToDto(It.IsAny<Quiz>()))
                .Returns<Quiz>(q => new QuizDto { Id = q.Id, Title = q.Title, Description = q.Description });

            var updated = await _service.UpdateAsync(1, dto);
            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated", updated!.Title);
            Assert.AreEqual("New description", updated.Description);

            var inDb = await _db.Quizzes.FindAsync(1);
            Assert.IsNotNull(inDb);
            Assert.AreEqual("Updated", inDb!.Title);
            Assert.AreEqual("New description", inDb.Description);

            var missing = await _service.UpdateAsync(99, dto);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test DeleteAsync - usuwa quiz dla istniejącego Id lub zwraca false dla nieistniejącego
        public async Task DeleteAsync_DeletesQuizOrReturnsFalse()
        {
            DbHelper.SeedQuizzes(_db);

            var deleted = await _service.DeleteAsync(1);
            Assert.IsTrue(deleted);
            var inDb = await _db.Quizzes.FindAsync(1);
            Assert.IsNull(inDb);

            var missing = await _service.DeleteAsync(99);
            Assert.IsFalse(missing);
        }

        [TestMethod]
        // Test GetQuestionsIdsAsync - zwraca poprawne Id pytań powiązanych z quizem
        public async Task GetQuestionsIdsAsync_ReturnsCorrectIds()
        {
            DbHelper.SeedQuizzes(_db);

            var ids = await _service.GetQuestionsIdsAsync(1);
            Assert.HasCount(2, ids);
            Assert.Contains(10, ids);
            Assert.Contains(20, ids);
        }
    }
}
