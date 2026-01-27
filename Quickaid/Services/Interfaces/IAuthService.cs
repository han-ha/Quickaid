using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obs³ugi logowania i rejestracji
    public interface IAuthService
    {
        // Rejestruje nowego u¿ytkownika
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);

        // Loguje u¿ytkownika
        Task<AuthResultDto> LoginAsync(LoginDto dto);
    }
}
