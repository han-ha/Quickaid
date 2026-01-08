using System.ComponentModel.DataAnnotations;

namespace Quickaid.Models.DTO
{
    public class RegisterDto
    {
        public string Username { get; set; } = "";

        [EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail")]
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";
    }
}
