using Quickaid.Data;
using Quickaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quickaid.Mapping.Interfaces;
using Quickaid.Models.DTO;

namespace Quickaid.Services
{
    public class QuizService(AppDbContext db, IQuizMapper mapper, IQuestionService questionService) : IQuizService
    {
        private readonly AppDbContext _db = db;
        private readonly IQuizMapper _mapper = mapper;
        private readonly IQuestionService _questionService = questionService;

        // pobranie wszystkich quizów
        public async Task<IEnumerable<QuizDto>> GetAllAsync()
        {
            var quizzes = await _db.Quizzes.ToListAsync();
            return quizzes.Select(q => _mapper.ToDto(q));
        }

        // pobranie quizu po id
        public async Task<QuizDto?> GetByIdAsync(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return null;
            return _mapper.ToDto(quiz);
        }

        // dodanie nowego quizu
        public async Task<QuizDto> AddAsync(QuizDto dto)
        {
            var quiz = _mapper.ToEntity(dto);
            quiz.CreatedAt = DateTime.UtcNow;

            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(quiz);
        }

        public async Task<QuizDto?> UpdateAsync(int id, QuizDto dto)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return null;

            quiz.Title = dto.Title;
            quiz.Description = dto.Description;

            await _db.SaveChangesAsync();
            return _mapper.ToDto(quiz);
        }

        // usuwanie z kaskadowym usuwaniem pytañ i ich odpowiedzi
        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var quiz = await _db.Quizzes.FindAsync(id);
                if (quiz == null) return false;

                // Pobierz pytania przypisane do tego quizu
                var questionIds = await _db.QuizQuestions
                    .Where(qq => qq.QuizId == id)
                    .Select(qq => qq.QuestionId)
                    .ToListAsync();

                // Usuñ powi¹zania w QuizQuestions i sam quiz
                _db.QuizQuestions.RemoveRange(_db.QuizQuestions.Where(qq => qq.QuizId == id));
                _db.Quizzes.Remove(quiz);
                await _db.SaveChangesAsync();

                // Delegacja usuwania pytañ do QuestionService
                foreach (var qId in questionIds)
                {
                    var isUsedInOtherQuizzes = await _db.QuizQuestions.AnyAsync(qq => qq.QuestionId == qId);
                    if (!isUsedInOtherQuizzes)
                    {
                        await _questionService.DeleteAsync(qId);
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
