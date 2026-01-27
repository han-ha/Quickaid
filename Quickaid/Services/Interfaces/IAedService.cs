using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obs³ugi AED
    public interface IAedService
    {
        // Zwraca wszystkie AED z bazy danych
        Task<IEnumerable<InternalAedDto>> GetInternalAedsAsync();

        // Zwraca AED po Id
        Task<InternalAedDto?> GetByIdAsync(int id);

        // Dodaje nowe AED
        Task<InternalAedDto> AddAsync(InternalAedDto dto);

        // Aktualizuje AED o podanym Id
        Task<InternalAedDto?> UpdateAsync(int id, InternalAedDto dto);

        // Usuwa AED po Id
        Task<bool> DeleteAsync(int id);

        // Zwraca po³¹czone AED (z bazy i z API)
        Task<IEnumerable<AedDto>> GetMergedAedsAsync();
    }
}
