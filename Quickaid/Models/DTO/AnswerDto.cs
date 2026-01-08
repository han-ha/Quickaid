namespace Quickaid.Models.DTO
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public required string AnswerText { get; set; }
	public bool IsCorrect { get; set; }
    }
}