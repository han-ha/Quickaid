using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IQuestionMapper
    {
        QuestionDto ToDto(Question entity);
        Question ToEntity(QuestionDto dto);
    }
}

