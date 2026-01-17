using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IInternalAedMapper
    {
        InternalAedDto ToDto(AedPoint entity);
        AedPoint ToEntity(InternalAedDto dto);
    }
}

