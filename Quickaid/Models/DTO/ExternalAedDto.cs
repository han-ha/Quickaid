using Quickaid.Enums;

namespace Quickaid.Models.DTO
{
    public class ExternalAedDto
    {
        public long ExternalId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Description { get; set; }
        public AedType Type { get; set; } = AedType.External;
    }
}
