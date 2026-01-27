using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    // Mapuje artyku³y miêdzy encj¹ a DTO
    public class ArticleMapper : IArticleMapper
    {
        // Konwertuje encjê Article na DTO
        public ArticleDto ToDto(Article entity)
        {
            return new ArticleDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content ?? ""
            };
        }

        // Konwertuje DTO na encjê Article
        public Article ToEntity(ArticleDto dto)
        {
            return new Article
            {
                Title = dto.Title,
                Content = dto.Content
            };
        }
    }
}
