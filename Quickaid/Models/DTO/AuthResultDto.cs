namespace Quickaid.Models.DTO
{
    // DTO dla wyniku logowania/rejestracji
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? Token { get; set; }
    }
}
