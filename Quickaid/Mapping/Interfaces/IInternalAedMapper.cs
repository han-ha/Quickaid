using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera AED wewnêtrznych (z bazy)
    public interface IInternalAedMapper
    {
        // Konwertuje encjê AedPoint na DTO wewnêtrzne
        InternalAedDto ToDto(AedPoint entity);

        // Konwertuje DTO wewnêtrzne na encjê AedPoint
        AedPoint ToEntity(InternalAedDto dto);
    }
}
