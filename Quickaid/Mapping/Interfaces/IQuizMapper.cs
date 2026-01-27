using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera quizów
    public interface IQuizMapper
    {
        // Konwertuje encjê Quiz na DTO
        QuizDto ToDto(Quiz entity);

        // Konwertuje DTO na encjê Quiz
        Quiz ToEntity(QuizDto dto);
    }
}
