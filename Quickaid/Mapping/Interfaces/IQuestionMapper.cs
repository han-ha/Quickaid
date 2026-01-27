using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera pytañ quizowych
    public interface IQuestionMapper
    {
        // Konwertuje encjê Question na DTO
        QuestionDto ToDto(Question entity);

        // Konwertuje DTO na encjê Question
        Question ToEntity(QuestionDto dto);
    }
}
