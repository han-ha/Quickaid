using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class InternalAedMapper : IInternalAedMapper
    {
        // Mapuje encjê AedPoint na InternalAedDto
        public InternalAedDto ToDto(AedPoint entity)
        {
            return new InternalAedDto
            {
                Id = entity.Id,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Description = entity.Description,
                Verified = entity.Verified
            };
        }

        // Mapuje InternalAedDto na encjê AedPoint
        public AedPoint ToEntity(InternalAedDto dto)
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
