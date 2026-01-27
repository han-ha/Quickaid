using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;

namespace Quickaid.Services
{
    // Serwis obsługujący rozwiązywanie quizów i wyników użytkowników
    public class QuizSolverService(AppDbContext db, IResultService resultService) : IQuizSolverService
    {
        private readonly AppDbContext _db = db;
        private readonly IResultService _resultService = resultService;

        // Oblicza wynik quizu na podstawie odpowiedzi użytkownika
        public async Task<int> CalculateScoreAsync(int quizId, Dictionary<int, int> userAnswers)
        {
            // Pobranie Id wszystkich pytań w quizie
            var questions = await _db.QuizQuestions
                .Where(qq => qq.QuizId == quizId)
                .Select(qq => qq.QuestionId)
                .ToListAsync();

            // Pobranie wszystkich odpowiedzi dla tych pytań
            var answers = await _db.Answers
                .Where(a => questions.Contains(a.QuestionId))
                .ToListAsync();

            int score = 0;

            // Iteracja po pytaniach i sprawdzenie poprawności odpowiedzi
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

        // Zapisuje wynik quizu użytkownika
        public async Task<ResultDto> SubmitQuizAsync(int userId, int quizId, Dictionary<int, int> userAnswers)
        {
            // Obliczenie punktów
            int score = await CalculateScoreAsync(quizId, userAnswers);

            var dto = new ResultDto
            {
                UserId = userId,
                QuizId = quizId,
                Score = score,
                CompletedAt = DateTime.UtcNow
            };

            // Zapisanie wyniku przez ResultService
            return await _resultService.AddAsync(dto);
        }

        // Pobiera ostatni wynik użytkownika dla danego quizu
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
