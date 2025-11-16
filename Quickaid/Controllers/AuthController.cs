using Microsoft.AspNetCore.Mvc;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // logika aplikacyjna dla autoryzacji i rejestracji
        private readonly IAuthService _authService = authService;

        // POST api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // walidacja danych wejœciowych
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // rejestracja nowego u¿ytkownika
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }

        // POST api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // walidacja danych wejœciowych
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // logowanie u¿ytkownika
            var result = await _authService.LoginAsync(dto);
            if (!result.Success) return Unauthorized(result.Message);
            return Ok(result);
        }
    }
}
