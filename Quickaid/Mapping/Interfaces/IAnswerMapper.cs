using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera odpowiedzi quizowych
    public interface IAnswerMapper
    {
        // Konwertuje encjê Answer na DTO
        AnswerDto ToDto(Answer entity);

        // Konwertuje DTO na encjê Answer
        Answer ToEntity(AnswerDto dto);
    }
}
