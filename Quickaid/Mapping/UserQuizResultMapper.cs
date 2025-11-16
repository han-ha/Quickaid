using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class UserQuizResultMapper : IUserQuizResultMapper
    {
        public ResultDto ToDto(UserQuizResult entity)
        {
            return new ResultDto
            {
                QuizId = entity.QuizId,
                UserId = entity.UserId,
                Score = entity.Score ?? 0,
                CompletedAt = entity.CompletedAt
            };
        }

        public UserQuizResult ToEntity(ResultDto dto)
        {
            return new UserQuizResult
            {
                QuizId = dto.QuizId,
                Score = dto.Score,
                CompletedAt = dto.CompletedAt
            };
        }
    }
}

