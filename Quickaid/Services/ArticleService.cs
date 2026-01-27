using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Services
{
    // Serwis obs³uguj¹cy artyku³y
    public class ArticleService(AppDbContext db, IArticleMapper mapper) : IArticleService
    {
        private readonly AppDbContext _db = db;
        private readonly IArticleMapper _mapper = mapper;

        // Zwraca wszystkie artyku³y
        public async Task<IEnumerable<ArticleDto>> GetAllAsync()
        {
            var entities = await _db.Articles.ToListAsync();
            return entities.Select(a => _mapper.ToDto(a));
        }

        // Zwraca artyku³ po Id
        public async Task<ArticleDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : _mapper.ToDto(entity);
        }

        // Dodaje nowy artyku³ przypisany do u¿ytkownika
        public async Task<ArticleDto> AddAsync(ArticleDto dto, int userId)
        {
            var entity = new Article
            {
                Title = dto.Title,
                Content = dto.Content,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Dodanie artyku³u do bazy
            _db.Articles.Add(entity);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        // Aktualizuje artyku³ o podanym Id
        public async Task<ArticleDto?> UpdateAsync(int id, ArticleDto dto)
        {
            var entity = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return null;

            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return _mapper.ToDto(entity);
        }

        // Usuwa artyku³ po Id
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return false;

            _db.Articles.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
