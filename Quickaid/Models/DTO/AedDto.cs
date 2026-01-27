using Quickaid.Enums;

namespace Quickaid.Models.DTO
{
    // DTO dla AED gotowego do wysłania do frontendu
    public class AedDto
    {
        public int? Id { get; set; } // Id w bazie (nullable, bo punkty z API mogą go nie mieć)
        public long? ExternalId { get; set; } // Id z API (nullable, bo punkty dodane przez usera go nie mają)
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Description { get; set; }
        public bool Verified { get; set; }
        public AedType Type { get; set; }
    }
}
