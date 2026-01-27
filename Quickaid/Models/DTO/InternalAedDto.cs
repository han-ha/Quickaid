using Quickaid.Enums;

namespace Quickaid.Models.DTO
{
    // DTO dla AED z bazy danych
    public class InternalAedDto
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Description { get; set; }
        public bool Verified { get; set; }
        public AedType Type { get; set; } = AedType.Internal;
    }
}
