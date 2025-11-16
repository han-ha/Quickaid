using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class QuizMapper : IQuizMapper
    {
        public QuizDto ToDto(Quiz entity)
        {
            return new QuizDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description ?? "",
                NumberOfQuestions = entity.NumberOfQuestions ?? 0
            };
        }

        public Quiz ToEntity(QuizDto dto)
        {
            return new Quiz
            {
                Title = dto.Title,
                Description = dto.Description,
                NumberOfQuestions = dto.NumberOfQuestions
            };
        }
    }
}

