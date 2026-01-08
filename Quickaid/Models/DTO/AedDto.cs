namespace Quickaid.Models.DTO
{
    public class AedDto
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Description { get; set; }
        public bool Verified { get; set; }
    }
}
