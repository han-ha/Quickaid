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
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Rejestruje nowego u¿ytkownika
        /// </summary>
        /// <param name="dto">Dane rejestracyjne u¿ytkownika</param>
        /// <returns>Informacje o wyniku rejestracji</returns>
        /// <response code="200">Rejestracja powiod³a siê</response>
        /// <response code="400">Niepoprawne dane wejœciowe lub problem z rejestracj¹</response>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }

        /// <summary>
        /// Loguje u¿ytkownika i zwraca token JWT
        /// </summary>
        /// <param name="dto">Dane logowania</param>
        /// <returns>Token JWT w przypadku powodzenia</returns>
        /// <response code="200">Logowanie powiod³o siê, zwrócono token</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        /// <response code="401">Niepoprawny login lub has³o</response>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);
            if (!result.Success) return Unauthorized(result.Message);
            return Ok(result);
        }
    }
}
