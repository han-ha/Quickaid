using Moq;
using Quickaid.Controllers;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace QuickaidBackendTests.ControllerTests
{
    [TestClass]
    public class AedControllerTests
    {
        private Mock<IAedService> _serviceMock;
        private AedController _controller;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IAedService>();

            // AedGeoJsonUtils nie jest używane w tych testach, dlatego jest null
            _controller = new AedController(_serviceMock.Object, null!);

            // Fake admin claim do kontrolera
            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new("id", "1"),
                new(ClaimTypes.Role, "admin")
            ], "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        // Test GET /api/aed dla pobrania wszystkich AED
        public async Task GetAllAedPoints_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetMergedAedsAsync())
                .ReturnsAsync(
                [
                    new AedDto { Id = 1, Latitude = 50, Longitude = 20, Description = "AED przy szkole", Verified = true },
                    new AedDto { Id = 2, Latitude = 51, Longitude = 21, Description = "AED w szpitalu", Verified = false }
                ]);

            var result = await _controller.GetAllAedPoints();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var list = ok!.Value as IEnumerable<AedDto>;
            Assert.AreEqual(2, list!.Count());
        }

        [TestMethod]
        // Test GET /api/aed/{id} dla istniejącego AED
        public async Task GetAedById_AedExists_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new InternalAedDto { Id = 1, Latitude = 50, Longitude = 20, Description = "AED przy szkole", Verified = true });

            var result = await _controller.GetAedById(1);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test GET /api/aed/{id} dla nieistniejącego AED
        public async Task GetAedById_AedNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((InternalAedDto?)null);

            var result = await _controller.GetAedById(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test POST /api/aed dla dodania AED
        public async Task AddAed_ValidAed_ReturnsCreatedAtAction()
        {
            var dto = new InternalAedDto { Latitude = 50, Longitude = 20, Description = "Nowe AED", Verified = true };
            _serviceMock.Setup(s => s.AddAsync(dto))
                .ReturnsAsync(new InternalAedDto { Id = 10, Latitude = 50, Longitude = 20, Description = "Nowe AED", Verified = true });

            var result = await _controller.AddAed(dto);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var created = (result as CreatedAtActionResult)!.Value as InternalAedDto;
            Assert.AreEqual(10, created!.Id);
        }

        [TestMethod]
        // Test PUT /api/aed/{id} dla aktualizacji istniejącego AED
        public async Task UpdateAed_AedExists_ReturnsOk()
        {
            var dto = new InternalAedDto { Latitude = 51, Longitude = 21, Description = "Zaktualizowane AED", Verified = false };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto))
                .ReturnsAsync(new InternalAedDto { Id = 1, Latitude = 51, Longitude = 21, Description = "Zaktualizowane AED", Verified = false });

            var result = await _controller.UpdateAed(1, dto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        // Test PUT /api/aed/{id} dla aktualizacji nieistniejącego AED
        public async Task UpdateAed_AedNotExists_ReturnsNotFound()
        {
            var dto = new InternalAedDto { Latitude = 51, Longitude = 21, Description = "Zaktualizowane AED", Verified = false };
            _serviceMock.Setup(s => s.UpdateAsync(99, dto))
                .ReturnsAsync((InternalAedDto?)null);

            var result = await _controller.UpdateAed(99, dto);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test DELETE /api/aed/{id} dla istniejącego AED
        public async Task DeleteAed_AedExists_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.DeleteAed(1);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        // Test DELETE /api/aed/{id} dla nieistniejącego AED
        public async Task DeleteAed_AedNotExists_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(99))
                .ReturnsAsync(false);

            var result = await _controller.DeleteAed(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        // Test GET /api/aed/internal do pobrania AED z bazy
        public async Task GetInternalAeds_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetInternalAedsAsync())
                .ReturnsAsync(
                [
                    new InternalAedDto { Id = 1, Latitude = 50, Longitude = 20, Description = "AED w szkole", Verified = true }
                ]);

            var result = await _controller.GetInternalAeds();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var list = ok!.Value as IEnumerable<InternalAedDto>;
            Assert.AreEqual(1, list!.Count());
        }
    }
}
