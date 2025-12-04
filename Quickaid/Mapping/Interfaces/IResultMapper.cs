using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IResultMapper
    {
        ResultDto ToDto(Result entity);
        Result ToEntity(ResultDto dto);
    }
}

