using Moq;
using Quickaid.Data;
using Quickaid.Mapping.Interfaces;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services;
using QuickaidBackendTests.TestHelpers;

namespace QuickaidBackendTests.ServiceTests
{
    [TestClass]
    public class AnswerServiceTests
    {
        private AppDbContext _db;
        private Mock<IAnswerMapper> _mapperMock;
        private AnswerService _service;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();
            DbHelper.SeedQuizzes(_db);

            // Mapowanie
            _mapperMock = new Mock<IAnswerMapper>();
            _mapperMock.Setup(m => m.ToDto(It.IsAny<Answer>()))
                .Returns<Answer>(a => new AnswerDto
                {
                    Id = a.Id,
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                });

            _service = new AnswerService(_db, _mapperMock.Object);
        }

        [TestMethod]
        // Test GetAllAsync - zwraca wszystkie odpowiedzi w bazie
        public async Task GetAllAsync_ReturnsAllAnswers()
        {
            var result = await _service.GetAllAsync();
            
            Assert.AreEqual(6, result.Count());
            Assert.IsTrue(result.Any(a => a.AnswerText == "A"));
        }

        [TestMethod]
        // Test GetByIdAsync - zwraca odpowiedź dla istniejącego Id lub null dla nieistniejącego
        public async Task GetByIdAsync_ReturnsAnswerOrNull()
        {
            var existing = await _service.GetByIdAsync(1);
            Assert.IsNotNull(existing);
            Assert.AreEqual("A", existing!.AnswerText);

            var missing = await _service.GetByIdAsync(99);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test AddAsync - dodaje nową odpowiedź do pytania i zwraca DTO
        public async Task AddAsync_AddsAnswerSuccessfully()
        {
            var dto = new AnswerDto { AnswerText = "C", IsCorrect = false };
            var result = await _service.AddAsync(dto, 10);

            Assert.IsNotNull(result);
            Assert.AreEqual("C", result.AnswerText);

            var inDb = await _db.Answers.FindAsync(result.Id);
            Assert.IsNotNull(inDb);
        }

        [TestMethod]
        // Test UpdateAsync - aktualizuje istniejącą odpowiedź lub zwraca null dla nieistniejącej
        public async Task UpdateAsync_UpdatesAnswerOrReturnsNull()
        {
            var dto = new AnswerDto { AnswerText = "Updated", IsCorrect = true };
            var updated = await _service.UpdateAsync(1, dto);

            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated", updated!.AnswerText);

            var missing = await _service.UpdateAsync(99, dto);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test DeleteAsync - usuwa odpowiedź i zmniejsza licznik odpowiedzi w pytaniu lub zwraca null dla nieistniejącej
        public async Task DeleteAsync_DeletesAnswerAndDecrementsQuestionCounter()
        {
            var question = await _db.Questions.FindAsync(10);
            int initialCount = question!.NumberOfAnswers ?? 0;

            var deleted = await _service.DeleteAsync(1);
            Assert.IsTrue(deleted);

            var inDb = await _db.Answers.FindAsync(1);
            Assert.IsNull(inDb);

            var updatedQuestion = await _db.Questions.FindAsync(10);
            Assert.AreEqual(initialCount - 1, updatedQuestion!.NumberOfAnswers);

            var missing = await _service.DeleteAsync(99);
            Assert.IsFalse(missing);
        }
    }
}
