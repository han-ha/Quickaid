using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Services
{
    public class AedService(AppDbContext db, IAedMapper mapper) : IAedService
    {
        private readonly AppDbContext _db = db;
        private readonly IAedMapper _mapper = mapper;

        public async Task<IEnumerable<AedDto>> GetAllAsync()
        {
            var entities = await _db.AedPoints.AsNoTracking().ToListAsync();
            return entities.Select(e => _mapper.ToDto(e));
        }

        public async Task<AedDto?> GetByIdAsync(int id)
        {
            var entity = await _db.AedPoints.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : _mapper.ToDto(entity);
        }

        public async Task<AedDto> AddAsync(AedDto dto)
        {
            var entity = _mapper.ToEntity(dto);
            entity.UpdatedAt = DateTime.UtcNow;

            _db.AedPoints.Add(entity);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        public async Task<AedDto?> UpdateAsync(int id, AedDto dto)
        {
            var entity = await _db.AedPoints.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return null;

            entity.Latitude = dto.Latitude;
            entity.Longitude = dto.Longitude;
            entity.Description = dto.Description;
            entity.Verified = dto.Verified;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return _mapper.ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.AedPoints.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return false;

            _db.AedPoints.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
