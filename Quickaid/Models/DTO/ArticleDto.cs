namespace Quickaid.Models.DTO
{
    // dane artyku³u edukacyjnego
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int CreatedBy { get; set; }
    }
}
