using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obs³ugi wyników quizów
    public interface IResultService
    {
        // Zwraca wszystkie wyniki
        Task<IEnumerable<ResultDto>> GetAllAsync();

        // Zwraca wyniki u¿ytkownika
        Task<IEnumerable<ResultDto>> GetByUserAsync(int userId);

        // Dodaje nowy wynik
        Task<ResultDto> AddAsync(ResultDto dto);

        // Zwraca wynik po Id
        Task<ResultDto?> GetByIdAsync(int id);

        // Aktualizuje wynik o podanym Id
        Task<ResultDto?> UpdateAsync(int id, ResultDto dto);

        // Usuwa wynik po Id
        Task<bool> DeleteAsync(int id);

        // Zwraca najlepszy wynik u¿ytkownika dla danego quizu
        Task<ResultDto?> GetBestResultForUserAsync(int userId, int quizId);
    }
}
