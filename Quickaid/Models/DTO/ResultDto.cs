namespace Quickaid.Models.DTO
{
    public class ResultDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
