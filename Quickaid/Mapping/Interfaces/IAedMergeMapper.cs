using Quickaid.Models.DTO;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera do łączenia AED wewnętrznych i zewnętrznych w AedDto
    public interface IAedMergeMapper
    {
        // Konwertuje wewnętrzne AED (z bazy) na AedDto
        AedDto ToDto(InternalAedDto internalDto);

        // Konwertuje zewnętrzne AED (z API) na AedDto
        AedDto ToDto(ExternalAedDto externalDto);
    }
}
