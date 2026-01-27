using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    // Encja mapuj¹ca tabelê articles w bazie danych
    [Table("articles")]
    public class Article
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("content")]
        public string? Content { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
