using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IAnswerMapper
    {
        AnswerDto ToDto(Answer entity);
        Answer ToEntity(AnswerDto dto);
    }
}

