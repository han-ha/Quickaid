using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IExternalAedMapper
    {
        AedPoint ToEntity(ExternalAedDto dto);
        ExternalAedDto ToDto(AedPoint entity);
    }
}
