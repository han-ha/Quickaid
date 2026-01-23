using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Services;
using QuickaidBackendTests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;

namespace QuickaidBackendTests.ServiceTests
{
    [TestClass]
    public class ResultServiceTests
    {
        private AppDbContext _db = null!;
        private ResultService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();
            DbHelper.SeedResults(_db);

            _service = new ResultService(_db);
        }

        [TestMethod]
        // Test GetAllAsync - zwraca wszystkie wyniki z bazy
        public async Task GetAllAsync_ReturnsAllResults()
        {
            var results = await _service.GetAllAsync();
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        // Test GetByUserAsync - zwraca tylko wyniki dla konkretnego użytkownika
        public async Task GetByUserAsync_ReturnsOnlyUserResults()
        {
            var results = await _service.GetByUserAsync(1);
            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.All(r => r.UserId == 1));
        }

        [TestMethod]
        // Test AddAsync - dodaje nowy wynik i zapisuje go w bazie
        public async Task AddAsync_AddsResult()
        {
            var dto = new ResultDto { UserId = 3, QuizId = 2, Score = 85 };
            var added = await _service.AddAsync(dto);

            Assert.IsNotNull(added);
            Assert.AreEqual(3, added.UserId);
            Assert.AreEqual(85, added.Score);

            var dbResult = await _db.UserQuizResults.FindAsync(added.Id);
            Assert.IsNotNull(dbResult);
        }

        [TestMethod]
        // Test GetByIdAsync - zwraca wynik po Id dla istniejącego wyniku
        public async Task GetByIdAsync_ReturnsCorrectResult()
        {
            var result = await _service.GetByIdAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual(90, result.Score);
        }

        [TestMethod]
        // Test UpdateAsync - aktualizuje istniejący wynik
        public async Task UpdateAsync_UpdatesResult()
        {
            var dto = new ResultDto { Score = 95 };
            var updated = await _service.UpdateAsync(2, dto);

            Assert.IsNotNull(updated);
            Assert.AreEqual(95, updated.Score);
            Assert.AreEqual(2, updated.Id);
        }

        [TestMethod]
        // Test DeleteAsync - usuwa wynik, jeśli istnieje
        public async Task DeleteAsync_RemovesResult()
        {
            var deleted = await _service.DeleteAsync(3);
            Assert.IsTrue(deleted);
            Assert.IsNull(await _db.UserQuizResults.FindAsync(3));
        }

        [TestMethod]
        // Test GetBestResultForUserAsync - zwraca najlepszy wynik użytkownika dla konkretnego quizu
        public async Task GetBestResultForUserAsync_ReturnsHighestScore()
        {
            var best = await _service.GetBestResultForUserAsync(1, 1);
            Assert.IsNotNull(best);
            Assert.AreEqual(90, best.Score);
        }
    }
}
