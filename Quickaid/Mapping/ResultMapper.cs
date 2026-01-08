using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class ResultMapper : IResultMapper
    {
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

