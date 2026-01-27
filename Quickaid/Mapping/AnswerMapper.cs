using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    // Mapuje odpowiedzi quizowe miêdzy encj¹ a DTO
    public class AnswerMapper : IAnswerMapper
    {
        // Konwertuje encjê Answer na DTO
        public AnswerDto ToDto(Answer entity)
        {
            return new AnswerDto
            {
                Id = entity.Id,
                AnswerText = entity.AnswerText ?? "",
                IsCorrect = entity.IsCorrect
            };
        }

        // Konwertuje DTO na encjê Answer
        public Answer ToEntity(AnswerDto dto)
        {
            return new Answer
            {
                AnswerText = dto.AnswerText,
                IsCorrect = dto.IsCorrect
            };
        }
    }
}
