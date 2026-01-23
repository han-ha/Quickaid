using Quickaid.Models.DTO;
using Quickaid.Mapping.Interfaces;
using Quickaid.Enums;

namespace Quickaid.Mapping
{
    public class AedMergeMapper : IAedMergeMapper
    {
        public AedDto ToDto(InternalAedDto internalDto)
        {
            return new AedDto
            {
                Id = internalDto.Id,
                ExternalId = null,
                Type = AedType.Internal,
                Latitude = internalDto.Latitude,
                Longitude = internalDto.Longitude,
                Description = internalDto.Description,
                Verified = internalDto.Verified
            };
        }

        public AedDto ToDto(ExternalAedDto externalDto)
        {
            return new AedDto
            {
                Id = null,
                ExternalId = externalDto.ExternalId,
                Type = AedType.External,
                Latitude = externalDto.Latitude,
                Longitude = externalDto.Longitude,
                Description = externalDto.Description,
                Verified = true
            };
        }

    }
}
