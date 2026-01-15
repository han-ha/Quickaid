using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Services
{
    public class ArticleService(AppDbContext db, IArticleMapper mapper) : IArticleService
    {
        private readonly AppDbContext _db = db;
        private readonly IArticleMapper _mapper = mapper;

        public async Task<IEnumerable<ArticleDto>> GetAllAsync()
        {
            var entities = await _db.Articles.ToListAsync();
            return entities.Select(a => _mapper.ToDto(a));
        }

        public async Task<ArticleDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : _mapper.ToDto(entity);
        }

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

            _db.Articles.Add(entity);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

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
