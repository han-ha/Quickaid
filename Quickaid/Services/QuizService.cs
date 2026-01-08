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

        public async Task<IEnumerable<QuizDto>> GetAllAsync()
        {
            var quizzes = await _db.Quizzes.ToListAsync();
            return quizzes.Select(q => _mapper.ToDto(q));
        }

        public async Task<QuizDto?> GetByIdAsync(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return null;

            var questions = await _questionService.GetByQuizIdAsync(id);

            return new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description ?? "",
                NumberOfQuestions = questions.Count,
                Questions = questions
            };
        }

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

        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return false;

            var questionIds = await _db.QuizQuestions
                .Where(qq => qq.QuizId == id)
                .Select(qq => qq.QuestionId)
                .ToListAsync();

            var usedElsewhere = await _db.QuizQuestions
                .Where(qq => qq.QuizId != id && questionIds.Contains(qq.QuestionId))
                .Select(qq => qq.QuestionId)
                .Distinct()
                .ToListAsync();

            var toDelete = questionIds.Except(usedElsewhere).ToList();

            _db.QuizQuestions.RemoveRange(_db.QuizQuestions.Where(qq => qq.QuizId == id));
            _db.Quizzes.Remove(quiz);

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<int>> GetQuestionsIdsAsync(int quizId)
        {
            return await _db.QuizQuestions
                .Where(qq => qq.QuizId == quizId)
                .Select(qq => qq.QuestionId)
                .ToListAsync();
        }
    }
}
