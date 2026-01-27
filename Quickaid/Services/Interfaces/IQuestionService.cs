using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obsługi pytań
    public interface IQuestionService
    {
        // Zwraca wszystkie pytania
        Task<IEnumerable<QuestionDto>> GetAllAsync();

        // Zwraca pytanie po Id
        Task<QuestionDto?> GetByIdAsync(int id);

        // Dodaje nowe pytanie
        Task<QuestionDto> AddAsync(QuestionDto dto);

        // Aktualizuje pytanie o podanym Id
        Task<QuestionDto?> UpdateAsync(int id, QuestionDto dto);

        // Usuwa pytanie z quizu
        Task<bool> DeleteAsync(int questionId, int quizId);

        // Zwraca pytania dla danego quizu
        Task<List<QuestionDto>> GetByQuizIdAsync(int quizId);

        // Dodaje pytanie do quizu
        Task<QuestionDto> AddToQuizAsync(int quizId, QuestionDto dto);
    }
}
