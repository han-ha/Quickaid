using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obsługi odpowiedzi
    public interface IAnswerService
    {
        // Zwraca wszystkie odpowiedzi
        Task<IEnumerable<AnswerDto>> GetAllAsync();

        // Zwraca odpowiedź po Id
        Task<AnswerDto?> GetByIdAsync(int id);

        // Dodaje nową odpowiedź do pytania
        Task<AnswerDto> AddAsync(AnswerDto dto, int questionId);

        // Aktualizuje odpowiedź o podanym Id
        Task<AnswerDto?> UpdateAsync(int id, AnswerDto dto);

        // Usuwa odpowiedź po Id
        Task<bool> DeleteAsync(int id);
    }
}
