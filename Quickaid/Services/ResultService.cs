using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;

namespace Quickaid.Services
{
    public class ResultService(AppDbContext db) : IResultService
    {
        private readonly AppDbContext _db = db;

        public async Task<IEnumerable<ResultDto>> GetAllAsync()
        {
            var results = await _db.UserQuizResults.ToListAsync();
            return results.Select(r => new ResultDto
            {
                Id = r.Id,
                UserId = r.UserId,
                QuizId = r.QuizId,
                Score = r.Score ?? 0,
                CompletedAt = r.CompletedAt
            });
        }

        public async Task<IEnumerable<ResultDto>> GetByUserAsync(int userId)
        {
            var results = await _db.UserQuizResults
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return results.Select(r => new ResultDto
            {
                Id = r.Id,
                UserId = r.UserId,
                QuizId = r.QuizId,
                Score = r.Score ?? 0,
                CompletedAt = r.CompletedAt
            });
        }

        public async Task<ResultDto> AddAsync(ResultDto dto)
        {
            var entity = new UserQuizResult
            {
                UserId = dto.UserId,
                QuizId = dto.QuizId,
                Score = dto.Score,
                CompletedAt = dto.CompletedAt == default ? DateTime.UtcNow : dto.CompletedAt
            };

            _db.UserQuizResults.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<ResultDto?> GetByIdAsync(int id)
        {
            var entity = await _db.UserQuizResults.FindAsync(id);
            if (entity == null) return null;

            return new ResultDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                QuizId = entity.QuizId,
                Score = entity.Score ?? 0,
                CompletedAt = entity.CompletedAt
            };
        }

        public async Task<ResultDto?> UpdateAsync(int id, ResultDto dto)
        {
            var entity = await _db.UserQuizResults.FindAsync(id);
            if (entity == null) return null;

            entity.Score = dto.Score;
            entity.CompletedAt = dto.CompletedAt == default ? DateTime.UtcNow : dto.CompletedAt;

            await _db.SaveChangesAsync();

            return new ResultDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                QuizId = entity.QuizId,
                Score = entity.Score ?? 0,
                CompletedAt = entity.CompletedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.UserQuizResults.FindAsync(id);
            if (entity == null) return false;

            _db.UserQuizResults.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
