using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IAedMapper
    {
        AedDto ToDto(AedPoint entity);
        AedPoint ToEntity(AedDto dto);
    }
}

