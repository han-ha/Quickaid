using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do rozwiązywania quizów i obliczania wyników
    public interface IQuizSolverService
    {
        // Zgłasza rozwiązanie quizu przez użytkownika
        Task<ResultDto> SubmitQuizAsync(int userId, int quizId, Dictionary<int, int> userAnswers);

        // Oblicza wynik quizu na podstawie odpowiedzi użytkownika
        Task<int> CalculateScoreAsync(int quizId, Dictionary<int, int> userAnswers);

        // Zwraca ostatni wynik użytkownika dla danego quizu
        Task<ResultDto?> GetLastResultAsync(int userId, int quizId);
    }
}