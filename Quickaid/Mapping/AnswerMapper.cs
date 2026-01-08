using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class AnswerMapper : IAnswerMapper
    {
        public AnswerDto ToDto(Answer entity)
        {
            return new AnswerDto
            {
                Id = entity.Id,
                AnswerText = entity.AnswerText ?? "",
                IsCorrect = entity.IsCorrect
            };
        }

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

