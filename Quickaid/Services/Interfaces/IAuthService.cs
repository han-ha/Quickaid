using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // logika autoryzacji i uwierzytelniania
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDto dto);
        Task<AuthResult> LoginAsync(LoginDto dto);
    }
}
