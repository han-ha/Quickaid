using System.ComponentModel.DataAnnotations.Schema;

namespace Quickaid.Models.Entities
{
    [Table("user_quiz_results")]
    public class UserQuizResult // TODO ujednoliciæ nazwy kolumn na Result?
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Column("score")]
        public int? Score { get; set; }

        [Column("completed_at")]
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
