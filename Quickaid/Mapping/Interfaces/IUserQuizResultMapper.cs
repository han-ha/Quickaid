using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IUserQuizResultMapper
    {
        ResultDto ToDto(UserQuizResult entity);
        UserQuizResult ToEntity(ResultDto dto);
    }
}

