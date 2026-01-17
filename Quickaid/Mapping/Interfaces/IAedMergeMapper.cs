using Quickaid.Models.DTO;

namespace Quickaid.Mapping.Interfaces
{
    public interface IAedMergeMapper
    {
        AedDto ToDto(InternalAedDto internalDto);
        AedDto ToDto(ExternalAedDto externalDto);
    }
}
