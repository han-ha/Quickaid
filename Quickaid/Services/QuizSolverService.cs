using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Services
{
    public interface IQuizSolverService
    {
        Task<ResultDto> SubmitQuizAsync(int userId, int quizId, Dictionary<int, int> userAnswers);
        Task<int> CalculateScoreAsync(int quizId, Dictionary<int, int> userAnswers);
    }

    public class QuizSolverService(AppDbContext db) : IQuizSolverService
    {
        private readonly AppDbContext _db = db;

        public async Task<int> CalculateScoreAsync(int quizId, Dictionary<int, int> userAnswers)
        {
            var questions = await _db.QuizQuestions
                .Where(qq => qq.QuizId == quizId)
                .Select(qq => qq.QuestionId)
                .ToListAsync();

            var answers = await _db.Answers
                .Where(a => questions.Contains(a.QuestionId))
                .ToListAsync();

            int score = 0;

            foreach (var qId in questions)
            {
                if (!userAnswers.TryGetValue(qId, out var selectedAnswerId))
                    continue;

                var answer = answers.FirstOrDefault(a => a.Id == selectedAnswerId);

                if (answer != null && answer.IsCorrect)
                    score++;
            }

            return score;
        }

        public async Task<ResultDto> SubmitQuizAsync(int userId, int quizId, Dictionary<int, int> userAnswers)
        {
            int score = await CalculateScoreAsync(quizId, userAnswers);

            var result = new Result
            {
                UserId = userId,
                QuizId = quizId,
                Score = score,
                CompletedAt = DateTime.UtcNow
            };

            _db.UserQuizResults.Add(result);
            await _db.SaveChangesAsync();

            return new ResultDto
            {
                Id = result.Id,
                UserId = result.UserId,
                QuizId = result.QuizId,
                Score = result.Score ?? 0,
                CompletedAt = result.CompletedAt
            };
        }

        public async Task<ResultDto?> GetLastResultAsync(int userId, int quizId)
        {
            var result = await _db.UserQuizResults
                .Where(r => r.UserId == userId && r.QuizId == quizId)
                .OrderByDescending(r => r.CompletedAt)
                .FirstOrDefaultAsync();

            if (result == null) return null;

            return new ResultDto
            {
                Id = result.Id,
                UserId = result.UserId,
                QuizId = result.QuizId,
                Score = result.Score ?? 0,
                CompletedAt = result.CompletedAt
            };
        }

    }
}
