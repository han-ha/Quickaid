using Microsoft.AspNetCore.Mvc;
using Moq;
using Quickaid.Controllers;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using System.Security.Claims;

namespace QuickaidApiTests.ControllerTests
{
    [TestClass]
    public class ArticlesControllerTests
    {
        private Mock<IArticleService> _serviceMock;
        private ArticlesController _controller;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IArticleService>();
            _controller = new ArticlesController(_serviceMock.Object);

            // Fake admin claim dla POST/PUT/DELETE
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
        // Test GET /api/articles pobierania wszystkich artykułów
        public async Task GetAll_ReturnsOkWithArticles()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(
                [
                    new ArticleDto { Id = 1, Title = "Article 1", Content = "Content 1" },
                    new ArticleDto { Id = 2, Title = "Article 2", Content = "Content 2" }
                ]);

            var result = await _controller.GetAll();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var articles = ok!.Value as IEnumerable<ArticleDto>;
            Assert.AreEqual(2, articles!.Count());
        }

        [TestMethod]
        // Test GET /api/articles/{id} dla istniejącego artykułu
        public async Task GetById_ArticleExists_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new ArticleDto { Id = 1, Title = "Article 1", Content = "Content 1" });

            var result = await _controller.GetById(1);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test GET /api/articles/{id} dla nieistniejącego artykułu
        public async Task GetById_ArticleNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((ArticleDto?)null);

            var result = await _controller.GetById(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test POST /api/articles dla dodania nowego artykułu
        public async Task Add_ValidArticle_ReturnsCreatedAtAction()
        {
            var dto = new ArticleDto { Title = "New Article", Content = "New Content" };
            _serviceMock.Setup(s => s.AddAsync(dto, 1))
                .ReturnsAsync(new ArticleDto { Id = 10, Title = "New Article", Content = "New Content" });

            var result = await _controller.Add(dto) as CreatedAtActionResult;
            Assert.IsNotNull(result);

            var created = result!.Value as ArticleDto;
            Assert.IsNotNull(created);
        }

        [TestMethod]
        // Test PUT /api/articles/{id} dla istniejącego artykułu
        public async Task Update_ArticleExists_ReturnsOk()
        {
            var dto = new ArticleDto { Title = "Updated", Content = "Updated Content" };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto))
                .ReturnsAsync(new ArticleDto { Id = 1, Title = "Updated", Content = "Updated Content" });

            var result = await _controller.Update(1, dto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test PUT /api/articles/{id} dla nieistniejącego artykułu
        public async Task Update_ArticleNotExists_ReturnsNotFound()
        {
            var dto = new ArticleDto { Title = "Updated", Content = "Updated Content" };
            _serviceMock.Setup(s => s.UpdateAsync(99, dto))
                .ReturnsAsync((ArticleDto?)null);

            var result = await _controller.Update(99, dto);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test DELETE /api/articles/{id} dla istniejącego artykułu
        public async Task Delete_ArticleExists_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        // Test DELETE /api/articles/{id} dla nieistniejącego artykułu
        public async Task Delete_ArticleNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(99))
                .ReturnsAsync(false);

            var result = await _controller.Delete(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
