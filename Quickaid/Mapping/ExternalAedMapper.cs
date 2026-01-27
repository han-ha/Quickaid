using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;
using System;

namespace Quickaid.Mapping
{
    public class ExternalAedMapper : IExternalAedMapper
    {
        // Mapuje ExternalAedDto na encję AedPoint
        public AedPoint ToEntity(ExternalAedDto dto)
        {
            return new AedPoint
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Description = dto.Description,
                Verified = true, // zawsze zweryfikowane z API
                ExternalId = dto.ExternalId,
                UpdatedAt = DateTime.UtcNow
            };
        }

        // Mapuje encję AedPoint na ExternalAedDto
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
