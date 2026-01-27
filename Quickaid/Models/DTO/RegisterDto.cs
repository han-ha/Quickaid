using System.ComponentModel.DataAnnotations;
using Quickaid.Utils;

namespace Quickaid.Models.DTO
{
    // DTO rejestracji
    public class RegisterDto
    {
        public string Username { get; set; } = "";

        [EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail")]
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        [ComparePasswords("Password", ErrorMessage = "Hasła muszą być takie same")]
        public string ConfirmPassword { get; set; } = "";
    }
}
