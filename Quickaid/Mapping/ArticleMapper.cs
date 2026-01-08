using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class ArticleMapper : IArticleMapper
    {
        public ArticleDto ToDto(Article entity)
        {
            return new ArticleDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content ?? "",
                CreatedBy = entity.CreatedBy ?? 0
            };
        }

        public Article ToEntity(ArticleDto dto)
        {
            return new Article
            {
                Title = dto.Title,
                Content = dto.Content,
                CreatedBy = dto.CreatedBy
            };
        }
    }
}

