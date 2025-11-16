using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IArticleMapper
    {
        ArticleDto ToDto(Article entity);
        Article ToEntity(ArticleDto dto);
    }
}

