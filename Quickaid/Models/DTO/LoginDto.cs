using System.ComponentModel.DataAnnotations;

namespace Quickaid.Models.DTO
{
    // TODO przemyslec strukture tego pliku i tych DTO
    // dane rejestracji
    public class RegisterDto
    {
        public string Username { get; set; } = "";

        [EmailAddress(ErrorMessage = "Nieprawid³owy adres e-mail")]
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";
    }

    // dane logowania
    public class LoginDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    // wynik logowania lub rejestracji
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? Token { get; set; }
    }
}
