using Quickaid.Data;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quickaid.Models.DTO;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Services
{
    public class AnswerService(AppDbContext db, IAnswerMapper mapper) : IAnswerService
    {
        private readonly AppDbContext _db = db;
        private readonly IAnswerMapper _mapper = mapper;

        public async Task<IEnumerable<AnswerDto>> GetAllAsync()
        {
            var entities = await _db.Answers.ToListAsync();
            return entities.Select(a => _mapper.ToDto(a));
        }

        public async Task<AnswerDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Answers.FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : _mapper.ToDto(entity);
        }

        public async Task<AnswerDto> AddAsync(AnswerDto dto, int questionId)
        {
            var entity = new Answer
            {
                QuestionId = questionId,
                AnswerText = dto.AnswerText,
                IsCorrect = false
            };

            _db.Answers.Add(entity);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        public async Task<AnswerDto?> UpdateAsync(int id, AnswerDto dto)
        {
            var entity = await _db.Answers.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return null;

            entity.AnswerText = dto.AnswerText;
            await _db.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var answer = await _db.Answers.FirstOrDefaultAsync(a => a.Id == id);
            if (answer == null) return false;

            // Pobierz powiązane pytanie
            var question = await _db.Questions.FindAsync(answer.QuestionId);

            // Usuń odpowiedź
            _db.Answers.Remove(answer);

            // Aktualizacja liczników w pytaniu
            if (question != null && question.NumberOfAnswers.HasValue)
            {
                question.NumberOfAnswers = Math.Max(0, question.NumberOfAnswers.Value - 1);
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
