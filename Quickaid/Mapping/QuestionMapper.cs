using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    // Mapuje pytania quizowe miêdzy encj¹ a DTO
    public class QuestionMapper : IQuestionMapper
    {
        // Konwertuje encjê Question na DTO
        public QuestionDto ToDto(Question entity)
        {
            return new QuestionDto
            {
                Id = entity.Id,
                QuestionText = entity.QuestionText,
                Answers = []
            };
        }

        // Konwertuje DTO na encjê Question
        public Question ToEntity(QuestionDto dto)
        {
            return new Question
            {
                QuestionText = dto.QuestionText
            };
        }
    }
}
