using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    // Mapuje wyniki quizów miêdzy encj¹ a DTO
    public class ResultMapper : IResultMapper
    {
        // Konwertuje encjê Result na DTO
        public ResultDto ToDto(Result entity)
        {
            return new ResultDto
            {
                QuizId = entity.QuizId,
                UserId = entity.UserId,
                Score = entity.Score ?? 0,
                CompletedAt = entity.CompletedAt
            };
        }

        // Konwertuje DTO na encjê Result
        public Result ToEntity(ResultDto dto)
        {
            return new Result
            {
                QuizId = dto.QuizId,
                Score = dto.Score,
                CompletedAt = dto.CompletedAt
            };
        }
    }
}
