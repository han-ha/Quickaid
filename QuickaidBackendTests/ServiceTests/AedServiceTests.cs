using Moq;
using Quickaid.Data;
using Quickaid.Services;
using Quickaid.Models.DTO;
using Quickaid.Mapping.Interfaces;
using QuickaidBackendTests.TestHelpers;
using Quickaid.Enums;

namespace QuickaidBackendTests.ServiceTests
{
    [TestClass]
    public class AedServiceTests
    {
        private AppDbContext _db;
        private Mock<IInternalAedMapper> _internalMapperMock;
        private Mock<IExternalAedMapper> _externalMapperMock;
        private Mock<IAedMergeMapper> _mergeMapperMock;
        private AedService _service;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();
            DbHelper.SeedAeds(_db);

            _internalMapperMock = new Mock<IInternalAedMapper>();
            _externalMapperMock = new Mock<IExternalAedMapper>();
            _mergeMapperMock = new Mock<IAedMergeMapper>();

            // Mapowanie
            _internalMapperMock.Setup(m => m.ToDto(It.IsAny<Quickaid.Models.Entities.AedPoint>()))
                .Returns<Quickaid.Models.Entities.AedPoint>(e => new InternalAedDto
                {
                    Id = e.Id,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    Description = e.Description,
                    Verified = e.Verified
                });

            _internalMapperMock.Setup(m => m.ToEntity(It.IsAny<InternalAedDto>()))
                .Returns<InternalAedDto>(dto => new Quickaid.Models.Entities.AedPoint
                {
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Description = dto.Description,
                    Verified = dto.Verified
                });

            _mergeMapperMock.Setup(m => m.ToDto(It.IsAny<InternalAedDto>()))
                .Returns<InternalAedDto>(dto => new AedDto
                {
                    Id = dto.Id,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Description = dto.Description,
                    Verified = dto.Verified,
                    Type = AedType.Internal
                });

            _mergeMapperMock.Setup(m => m.ToDto(It.IsAny<ExternalAedDto>()))
                .Returns<ExternalAedDto>(dto => new AedDto
                {
                    ExternalId = dto.ExternalId,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Description = dto.Description,
                    Verified = true,
                    Type = AedType.External
                });

            _service = new AedService(
                _db,
                _internalMapperMock.Object,
                _externalMapperMock.Object,
                _mergeMapperMock.Object
            );
        }

        [TestMethod]
        // Test GetInternalAedsAsync - sprawdza czy serwis zwraca wszystkie AED z bazy
        public async Task GetInternalAedsAsync_ReturnsAllInternalAeds()
        {
            var result = await _service.GetInternalAedsAsync();

            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.Any(a => a.Description == "AED 1 w centrum"));
        }

        [TestMethod]
        // Test GetByIdAsync - zwraca AED dla istniejącego id, null dla nieistniejącego
        public async Task GetByIdAsync_ReturnsAedOrNull()
        {
            var existing = await _service.GetByIdAsync(1);
            Assert.IsNotNull(existing);
            Assert.AreEqual("AED 1 w centrum", existing!.Description);

            var missing = await _service.GetByIdAsync(99);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test AddAsync - dodaje nowe AED do bazy i zwraca DTO
        public async Task AddAsync_AddsAedSuccessfully()
        {
            var dto = new InternalAedDto
            {
                Latitude = 50.1m,
                Longitude = 19.9m,
                Description = "Nowe AED",
                Verified = true
            };

            var result = await _service.AddAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Nowe AED", result.Description);

            var inDb = await _db.AedPoints.FindAsync(result.Id);
            Assert.IsNotNull(inDb);
        }

        [TestMethod]
        // Test UpdateAsync - aktualizuje istniejące AED lub zwraca null dla nieistniejącego
        public async Task UpdateAsync_UpdatesAedOrReturnsNull()
        {
            var dto = new InternalAedDto
            {
                Latitude = 50.2m,
                Longitude = 20.2m,
                Description = "Zaktualizowane AED",
                Verified = false
            };

            var updated = await _service.UpdateAsync(1, dto);
            Assert.IsNotNull(updated);
            Assert.AreEqual("Zaktualizowane AED", updated!.Description);

            var missing = await _service.UpdateAsync(99, dto);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test DeleteAsync - usuwa AED dla istniejącego id lub zwraca false, jeśli nie ma takiego AED
        public async Task DeleteAsync_DeletesAedOrReturnsFalse()
        {
            var deleted = await _service.DeleteAsync(1);
            Assert.IsTrue(deleted);

            var inDb = await _db.AedPoints.FindAsync(1);
            Assert.IsNull(inDb);

            var missing = await _service.DeleteAsync(99);
            Assert.IsFalse(missing);
        }
    }
}
