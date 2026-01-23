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
    public class ArticleServiceTests
    {
        private AppDbContext _db = null!;
        private Mock<IArticleMapper> _mapperMock = null!;
        private ArticleService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();
            DbHelper.SeedArticles(_db);

            _mapperMock = new Mock<IArticleMapper>();
            _mapperMock.Setup(m => m.ToDto(It.IsAny<Article>()))
                .Returns<Article>(a => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content
                });

            _service = new ArticleService(_db, _mapperMock.Object);
        }

        [TestMethod]
        // Test GetAllAsync - zwraca wszystkie artykuły z bazy
        public async Task GetAllAsync_ReturnsAllArticles()
        {
            var result = await _service.GetAllAsync();

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(a => a.Title == "Article 1"));
            Assert.IsTrue(result.Any(a => a.Title == "Article 2"));
        }

        [TestMethod]
        // Test GetByIdAsync - zwraca artykuł dla istniejącego Id lub null dla nieistniejącego
        public async Task GetByIdAsync_ReturnsArticleOrNull()
        {
            var existing = await _service.GetByIdAsync(1);
            Assert.IsNotNull(existing);
            Assert.AreEqual("Article 1", existing!.Title);

            var missing = await _service.GetByIdAsync(99);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test AddAsync - dodaje nowy artykuł i zwraca DTO
        public async Task AddAsync_AddsArticleSuccessfully()
        {
            var dto = new ArticleDto { Title = "New Article", Content = "New Content" };
            var result = await _service.AddAsync(dto, 5); // Dodany przez usera o id 5

            Assert.IsNotNull(result);
            Assert.AreEqual("New Article", result.Title);

            var inDb = await _db.Articles.FindAsync(result.Id);
            Assert.IsNotNull(inDb);
        }

        [TestMethod]
        // Test UpdateAsync - aktualizuje istniejący artykuł lub zwraca null dla nieistniejącego
        public async Task UpdateAsync_UpdatesArticleOrReturnsNull()
        {
            var dto = new ArticleDto { Title = "Updated", Content = "Updated Content" };

            var updated = await _service.UpdateAsync(1, dto);
            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated", updated!.Title);

            var missing = await _service.UpdateAsync(99, dto);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test DeleteAsync - usuwa artykuł lub zwraca false, jeśli nie istnieje
        public async Task DeleteAsync_DeletesArticleOrReturnsFalse()
        {
            var deleted = await _service.DeleteAsync(1);
            Assert.IsTrue(deleted);

            var inDb = await _db.Articles.FindAsync(1);
            Assert.IsNull(inDb);

            var missing = await _service.DeleteAsync(99);
            Assert.IsFalse(missing);
        }
    }
}
