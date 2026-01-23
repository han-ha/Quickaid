using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Quickaid.Mapping.Interfaces;
using Quickaid.Utils;
using Quickaid.Enums;

namespace Quickaid.Services
{
    public class AedService(
        AppDbContext db,
        IInternalAedMapper internalMapper,
        IExternalAedMapper externalMapper,
        IAedMergeMapper mergeMapper) : IAedService
    {
        private readonly AppDbContext _db = db;
        private readonly IInternalAedMapper _internalMapper = internalMapper;
        private readonly IAedMergeMapper _mergeMapper = mergeMapper;
        private readonly IExternalAedMapper _externalMapper = externalMapper;

        public async Task<IEnumerable<InternalAedDto>> GetInternalAedsAsync()
        {
            var entities = await _db.AedPoints.AsNoTracking().ToListAsync();
            return entities.Select(e => _internalMapper.ToDto(e));
        }

        public async Task<InternalAedDto?> GetByIdAsync(int id)
        {
            var entity = await _db.AedPoints.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : _internalMapper.ToDto(entity);
        }

        public async Task<InternalAedDto> AddAsync(InternalAedDto dto)
        {
            var entity = _internalMapper.ToEntity(dto);
            entity.UpdatedAt = DateTime.UtcNow;

            _db.AedPoints.Add(entity);
            await _db.SaveChangesAsync();

            return _internalMapper.ToDto(entity);
        }

        public async Task<InternalAedDto?> UpdateAsync(int id, InternalAedDto dto)
        {
            var entity = await _db.AedPoints.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return null;

            entity.Latitude = dto.Latitude;
            entity.Longitude = dto.Longitude;
            entity.Description = dto.Description;
            entity.Verified = dto.Verified;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return _internalMapper.ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.AedPoints.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return false;

            _db.AedPoints.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AedDto>> GetMergedAedsAsync()
        {
            // Pobierz AED z API
            var externalAeds = await new AedGeoJsonUtils().FetchExternalAedsAsync();

            // Pobierz z bazy tylko potrzebne wiersze
            var editedAedPoints = await _db.AedPoints
                .Where(a => a.ExternalId.HasValue)
                .AsNoTracking()
                .ToListAsync();

            var addedAedPoints = await _db.AedPoints
                .Where(a => !a.ExternalId.HasValue)
                .AsNoTracking()
                .ToListAsync();

            // S³ownik edytowanych AED po ExternalId
            var editedByExternalId = editedAedPoints.ToDictionary(a => a.ExternalId!.Value, a => a);

            var merged = new List<AedDto>();

            // Po³¹cz API z editedAedPoints
            foreach (var extAed in externalAeds)
            {
                if (editedByExternalId.TryGetValue(extAed.ExternalId, out var dbEntity))
                {
                    // Punkt z API istnieje w bazie i zosta³ nadpisany przez usera -> COMBINED
                    var dto = _mergeMapper.ToDto(_internalMapper.ToDto(dbEntity));
                    dto.Type = AedType.Combined;
                    merged.Add(dto);
                }
                else
                {
                    // Punkt tylko z API -> EXTERNAL
                    merged.Add(_mergeMapper.ToDto(extAed));
                }
            }

            // Dodaj AED dodane przez u¿ytkowników (bez ExternalId) -> INTERNAL
            foreach (var added in addedAedPoints)
            {
                merged.Add(_mergeMapper.ToDto(_internalMapper.ToDto(added)));
            }

            return merged;
        }

    }
}
