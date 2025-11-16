using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    [Table("aed_points")]
    public class AedPoint
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }

        [Column("longitude")]
        public decimal Longitude { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("added_by")]
        public int? AddedBy { get; set; }

        [Column("verified")]
        public bool Verified { get; set; } = false;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
