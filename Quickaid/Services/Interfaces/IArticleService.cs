using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDto>> GetAllAsync();
        Task<ArticleDto?> GetByIdAsync(int id);
        Task<ArticleDto> AddAsync(ArticleDto dto, int userId);
        Task<ArticleDto?> UpdateAsync(int id, ArticleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
