using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Services
{
    public class QuestionService(AppDbContext context, IQuestionMapper mapper, IAnswerMapper answerMapper) : IQuestionService
    {
        private readonly AppDbContext _context = context;
        private readonly IQuestionMapper _mapper = mapper;
        private readonly IAnswerMapper _answerMapper = answerMapper;

        public async Task<IEnumerable<QuestionDto>> GetAllAsync()
        {
            var questions = await _context.Questions.ToListAsync();
            var result = new List<QuestionDto>();

            foreach (var q in questions)
            {
                var answers = await _context.Answers
                    .Where(a => a.QuestionId == q.Id)
                    .ToListAsync();

                result.Add(new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Answers = [.. answers.Select(a => _answerMapper.ToDto(a))]
                });
            }

            return result;
        }

        public async Task<QuestionDto?> GetByIdAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return null;

            var answers = await _context.Answers
                .Where(a => a.QuestionId == id)
                .ToListAsync();

            return new QuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                Answers = [.. answers.Select(a => _answerMapper.ToDto(a))]
            };
        }

        public async Task<QuestionDto> AddAsync(QuestionDto dto)
        {
            var entity = _mapper.ToEntity(dto);
            _context.Questions.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        public async Task<QuestionDto?> UpdateAsync(int id, QuestionDto dto)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return null;

            question.QuestionText = dto.QuestionText;
            question.NumberOfAnswers = dto.Answers?.Count ?? 0;

            await _context.SaveChangesAsync();
            return _mapper.ToDto(question);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var question = await _context.Questions.FindAsync(id);
                if (question == null) return false;

                // Pobierz wszystkie quizy powiązane z tym pytaniem
                var quizLinks = await _context.QuizQuestions
                    .Where(qq => qq.QuestionId == id)
                    .ToListAsync();

                // Usuń powiązania w QuizQuestions
                _context.QuizQuestions.RemoveRange(quizLinks);

                // Aktualizacja liczników w quizach
                foreach (var link in quizLinks)
                {
                    var quiz = await _context.Quizzes.FindAsync(link.QuizId);
                    if (quiz != null && quiz.NumberOfQuestions.HasValue)
                    {
                        quiz.NumberOfQuestions = Math.Max(0, quiz.NumberOfQuestions.Value - 1);
                    }
                }

                // Usuń wszystkie odpowiedzi pytania, jeśli nie są użyte do innego pytania
                var answers = await _context.Answers
                    .Where(a => a.QuestionId == id)
                    .ToListAsync();
                _context.Answers.RemoveRange(answers);

                // Usuń pytanie
                _context.Questions.Remove(question);

                await _context.SaveChangesAsync();
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
