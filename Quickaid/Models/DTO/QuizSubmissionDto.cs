namespace Quickaid.Models.DTO
{
    public class QuizSubmissionDto
    {
        public int QuizId { get; set; }
        public Dictionary<int, int> Answers { get; set; } = new();
    }
}
