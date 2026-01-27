namespace Quickaid.Models.DTO
{
    // DTO dla odpowiedzi
    public class AnswerDto
    {
        public int Id { get; set; }
        public required string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}