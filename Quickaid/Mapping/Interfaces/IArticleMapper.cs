using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera artyku³ów
    public interface IArticleMapper
    {
        // Konwertuje encjê Article na DTO
        ArticleDto ToDto(Article entity);

        // Konwertuje DTO na encjê Article
        Article ToEntity(ArticleDto dto);
    }
}
