using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    // Encja mapuj¹ca tabelê answers w bazie danych
    [Table("answers")]
    public class Answer
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }

        [Column("answer_text")]
        public string? AnswerText { get; set; }

        [Column("is_correct")]
        public bool IsCorrect { get; set; } = false;
    }
}
