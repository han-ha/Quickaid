using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obs³ugi quizów
    public interface IQuizService
    {
        // Zwraca wszystkie quizy
        Task<IEnumerable<QuizDto>> GetAllAsync();

        // Zwraca quiz po Id
        Task<QuizDto?> GetByIdAsync(int id);

        // Dodaje nowy quiz
        Task<QuizDto> AddAsync(QuizDto dto);

        // Aktualizuje quiz o podanym Id
        Task<QuizDto?> UpdateAsync(int id, QuizDto dto);

        // Usuwa quiz po Id
        Task<bool> DeleteAsync(int id);

        // Zwraca listê Id pytañ w quizie
        Task<List<int>> GetQuestionsIdsAsync(int quizId);
    }
}
