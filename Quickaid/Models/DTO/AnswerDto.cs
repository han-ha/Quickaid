namespace Quickaid.Models.DTO
{
    // dane odpowiedzi na pytanie
    public class AnswerDto
    {
        public int Id { get; set; }
        public required string AnswerText { get; set; }
    }
}