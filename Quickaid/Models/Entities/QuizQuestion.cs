using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    // Encja mapuj¹ca tabelê quiz_questions w bazie danych

    [Table("quiz_questions")]
    public class QuizQuestion
    {
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }
    }
}
