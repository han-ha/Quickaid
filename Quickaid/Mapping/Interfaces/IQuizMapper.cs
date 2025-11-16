using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IQuizMapper
    {
        QuizDto ToDto(Quiz entity);
        Quiz ToEntity(QuizDto dto);
    }
}

