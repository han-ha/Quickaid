using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera AED zewnętrznych
    public interface IExternalAedMapper
    {
        // Konwertuje DTO AED zewnętrznego na encję AedPoint
        AedPoint ToEntity(ExternalAedDto dto);

        // Konwertuje encję AedPoint na DTO AED zewnętrznego
        ExternalAedDto ToDto(AedPoint entity);
    }
}
