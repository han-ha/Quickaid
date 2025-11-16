using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class QuestionMapper : IQuestionMapper
    {
        public QuestionDto ToDto(Question entity)
        {
            return new QuestionDto
            {
                Id = entity.Id,
                QuestionText = entity.QuestionText,
                Answers = []
            };
        }

        public Question ToEntity(QuestionDto dto)
        {
            return new Question
            {
                QuestionText = dto.QuestionText
            };
        }
    }
}

