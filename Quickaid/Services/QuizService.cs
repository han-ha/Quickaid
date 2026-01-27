using Quickaid.Data;
using Quickaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quickaid.Mapping.Interfaces;
using Quickaid.Models.DTO;

namespace Quickaid.Services
{
    // Serwis obs³uguj¹cy quizy i powi¹zania pytañ z quizami
    public class QuizService(AppDbContext db, IQuizMapper mapper, IQuestionService questionService) : IQuizService
    {
        private readonly AppDbContext _db = db;
        private readonly IQuizMapper _mapper = mapper;
        private readonly IQuestionService _questionService = questionService;

        // Zwraca wszystkie quizy
        public async Task<IEnumerable<QuizDto>> GetAllAsync()
        {
            var quizzes = await _db.Quizzes.ToListAsync();
            return quizzes.Select(q => _mapper.ToDto(q));
        }

        // Zwraca quiz po Id wraz z pytaniami
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

        // Dodaje nowy quiz
        public async Task<QuizDto> AddAsync(QuizDto dto)
        {
            var quiz = _mapper.ToEntity(dto);
            quiz.CreatedAt = DateTime.UtcNow;

            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(quiz);
        }

        // Aktualizuje quiz o podanym Id
        public async Task<QuizDto?> UpdateAsync(int id, QuizDto dto)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return null;

            quiz.Title = dto.Title;
            quiz.Description = dto.Description;

            await _db.SaveChangesAsync();
            return _mapper.ToDto(quiz);
        }

        // Usuwa quiz i powi¹zania pytañ z tym quizem
        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return false;

            // Pobranie wszystkich pytañ przypisanych do tego quizu
            var questionIds = await _db.QuizQuestions
                .Where(qq => qq.QuizId == id)
                .Select(qq => qq.QuestionId)
                .ToListAsync();

            // Sprawdzenie, które pytania s¹ u¿ywane w innych quizach
            var usedElsewhere = await _db.QuizQuestions
                .Where(qq => qq.QuizId != id && questionIds.Contains(qq.QuestionId))
                .Select(qq => qq.QuestionId)
                .Distinct()
                .ToListAsync();

            var toDelete = questionIds.Except(usedElsewhere).ToList();

            // Usuñ powi¹zania z quizem
            _db.QuizQuestions.RemoveRange(_db.QuizQuestions.Where(qq => qq.QuizId == id));

            // Usuñ sam quiz
            _db.Quizzes.Remove(quiz);

            await _db.SaveChangesAsync();
            return true;
        }

        // Zwraca listê Id pytañ dla danego quizu
        public async Task<List<int>> GetQuestionsIdsAsync(int quizId)
        {
            return await _db.QuizQuestions
                .Where(qq => qq.QuizId == quizId)
                .Select(qq => qq.QuestionId)
                .ToListAsync();
        }
    }
}
