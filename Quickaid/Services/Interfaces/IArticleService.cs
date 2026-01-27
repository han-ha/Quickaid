using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obs³ugi artyku³ów
    public interface IArticleService
    {
        // Zwraca wszystkie artyku³y
        Task<IEnumerable<ArticleDto>> GetAllAsync();

        // Zwraca artyku³ po Id
        Task<ArticleDto?> GetByIdAsync(int id);

        // Dodaje nowy artyku³ dla u¿ytkownika
        Task<ArticleDto> AddAsync(ArticleDto dto, int userId);

        // Aktualizuje artyku³ o podanym Id
        Task<ArticleDto?> UpdateAsync(int id, ArticleDto dto);

        // Usuwa artyku³ po Id
        Task<bool> DeleteAsync(int id);
    }
}
