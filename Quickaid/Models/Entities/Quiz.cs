using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    // Encja mapuj¹ca tabelê quizzes w bazie danych
    [Table("quizzes")]
    public class Quiz
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("number_of_questions")]
        public int? NumberOfQuestions { get; set; }

        [Column("max_score")]
        public int? MaxScore { get; set; }
    }
}
