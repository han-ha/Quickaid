namespace Quickaid.Models.DTO
{

    // DTO odpowiedzi użytkownika do quizu
    public class QuizSubmissionDto
    {
        public int QuizId { get; set; }
        public Dictionary<int, int> Answers { get; set; } = new();
    }
}
