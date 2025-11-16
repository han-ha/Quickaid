using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class AedMapper : IAedMapper
    {
        public AedDto ToDto(AedPoint entity)
        {
            return new AedDto
            {
                Id = entity.Id,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Description = entity.Description,
                Verified = entity.Verified
            };
        }

        public AedPoint ToEntity(AedDto dto)
        {
            return new AedPoint
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Description = dto.Description,
                Verified = dto.Verified
            };
        }
    }
}
