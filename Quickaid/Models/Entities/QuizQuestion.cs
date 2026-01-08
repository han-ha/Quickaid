using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    [Table("quiz_questions")]
    public class QuizQuestion
    {
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }
    }
}
