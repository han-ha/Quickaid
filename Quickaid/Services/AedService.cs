using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Quickaid.Mapping.Interfaces;
using Quickaid.Utils;
using Quickaid.Enums;

namespace Quickaid.Services
{
    // Serwis obs³uguj¹cy AED
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

        // Zwraca wszystkie AED z bazy danych
        public async Task<IEnumerable<InternalAedDto>> GetInternalAedsAsync()
        {
            var entities = await _db.AedPoints.AsNoTracking().ToListAsync();
            return entities.Select(e => _internalMapper.ToDto(e));
        }

        // Zwraca AED po Id
        public async Task<InternalAedDto?> GetByIdAsync(int id)
        {
            var entity = await _db.AedPoints.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : _internalMapper.ToDto(entity);
        }

        // Dodaje nowe AED do bazy
        public async Task<InternalAedDto> AddAsync(InternalAedDto dto)
        {
            var entity = _internalMapper.ToEntity(dto);
            entity.UpdatedAt = DateTime.UtcNow;

            _db.AedPoints.Add(entity);
            await _db.SaveChangesAsync();

            return _internalMapper.ToDto(entity);
        }

        // Aktualizuje AED o podanym Id
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

        // Usuwa AED po Id
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.AedPoints.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return false;

            _db.AedPoints.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        // Zwraca po³¹czon¹ listê AED z bazy i zewnêtrznego API
        public async Task<IEnumerable<AedDto>> GetMergedAedsAsync()
        {
            // Pobierz AED z API
            var externalAeds = await new AedGeoJsonUtils().FetchExternalAedsAsync();

            // Pobierz AED z bazy
            var internalAeds = await _db.AedPoints.AsNoTracking().ToListAsync();

            var merged = new List<AedDto>();

            // Konwertujemy i dodajemy AED z API do listy wynikowej
            foreach (var ext in externalAeds)
            {
                merged.Add(_mergeMapper.ToDto(ext));
            }

            // Konwertujemy i dodajemy AED z bazy do listy wynikowej
            foreach (var internalAed in internalAeds)
            {
                merged.Add(_mergeMapper.ToDto(_internalMapper.ToDto(internalAed)));
            }
            return merged;
        }
    }
}
