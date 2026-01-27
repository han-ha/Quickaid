namespace Quickaid.Models.DTO
{
    // DTO dla pytania
    public class QuestionDto
    {
        public int Id { get; set; }
        public required string QuestionText { get; set; }
        public required List<AnswerDto> Answers { get; set; }
    }
}
