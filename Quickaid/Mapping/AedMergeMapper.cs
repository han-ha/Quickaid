using Quickaid.Models.DTO;
using Quickaid.Mapping.Interfaces;

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
                Latitude = externalDto.Latitude,
                Longitude = externalDto.Longitude,
                Description = externalDto.Description,
                Verified = true
            };
        }
    }
}
