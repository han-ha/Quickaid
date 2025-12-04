using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IQuizSolverService
    {
        Task<ResultDto> SubmitQuizAsync(int userId, int quizId, Dictionary<int, int> userAnswers);
        Task<int> CalculateScoreAsync(int quizId, Dictionary<int, int> userAnswers);
        Task<ResultDto?> GetLastResultAsync(int userId, int quizId);

    }
}