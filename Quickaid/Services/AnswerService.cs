using Quickaid.Data;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quickaid.Models.DTO;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Services
{
    // Serwis obsługujący odpowiedzi na pytania
    public class AnswerService(AppDbContext db, IAnswerMapper mapper) : IAnswerService
    {
        private readonly AppDbContext _db = db;
        private readonly IAnswerMapper _mapper = mapper;

        // Zwraca wszystkie odpowiedzi
        public async Task<IEnumerable<AnswerDto>> GetAllAsync()
        {
            var entities = await _db.Answers.ToListAsync();
            return entities.Select(a => _mapper.ToDto(a));
        }

        // Zwraca odpowiedź po Id
        public async Task<AnswerDto?> GetByIdAsync(int id)
        {
            var entity = await _db.Answers.FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : _mapper.ToDto(entity);
        }

        // Dodaje nową odpowiedź do pytania
        public async Task<AnswerDto> AddAsync(AnswerDto dto, int questionId)
        {
            var entity = new Answer
            {
                QuestionId = questionId,
                AnswerText = dto.AnswerText,
                IsCorrect = dto.IsCorrect
            };

            _db.Answers.Add(entity);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        // Aktualizuje odpowiedź o podanym Id
        public async Task<AnswerDto?> UpdateAsync(int id, AnswerDto dto)
        {
            var entity = await _db.Answers.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null) return null;

            entity.AnswerText = dto.AnswerText;
            entity.IsCorrect = dto.IsCorrect;
            await _db.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        // Usuwa odpowiedź po Id
        public async Task<bool> DeleteAsync(int id)
        {
            // Znajdź odpowiedź
            var answer = await _db.Answers.FirstOrDefaultAsync(a => a.Id == id);
            if (answer == null) return false;

            // Pobierz powiązane pytanie
            var question = await _db.Questions.FindAsync(answer.QuestionId);

            // Usuń odpowiedź
            _db.Answers.Remove(answer);

            // Zaktualizuj licznik odpowiedzi w pytaniu
            if (question != null && question.NumberOfAnswers.HasValue)
            {
                question.NumberOfAnswers = Math.Max(0, (question.NumberOfAnswers ?? 0) - 1);
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
