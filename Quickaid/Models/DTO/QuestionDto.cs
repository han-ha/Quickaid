namespace Quickaid.Models.DTO
{
    // dane pytania
    public class QuestionDto
    {
        public int Id { get; set; }
        public required string QuestionText { get; set; }
        public required List<AnswerDto> Answers { get; set; }
    }
}
