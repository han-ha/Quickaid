using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;
using System;

namespace Quickaid.Mapping
{
    public class ExternalAedMapper : IExternalAedMapper
    {
        public AedPoint ToEntity(ExternalAedDto dto)
        {
            return new AedPoint
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Description = dto.Description,
                Verified = true,
                ExternalId = dto.ExternalId,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public ExternalAedDto ToDto(AedPoint entity)
        {
            return new ExternalAedDto
            {
                ExternalId = entity.ExternalId!.Value,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Description = entity.Description
            };
        }
    }
}
