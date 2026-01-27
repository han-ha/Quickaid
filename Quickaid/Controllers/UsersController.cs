using Microsoft.AspNetCore.Mvc;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Quickaid.Utils;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Pobiera wszystkich u¿ytkowników
        /// </summary>
        /// <returns>Lista u¿ytkowników</returns>
        /// <response code="200">Zwrócono listê u¿ytkowników</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania u¿ytkowników: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera u¿ytkownika po ID
        /// </summary>
        /// <param name="id">ID u¿ytkownika</param>
        /// <returns>Pojedynczy u¿ytkownik</returns>
        /// <response code="200">Zwrócono u¿ytkownika</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="404">Nie znaleziono u¿ytkownika o podanym ID</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null) return NotFound("Nie znaleziono u¿ytkownika.");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania u¿ytkownika: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera dane zalogowanego u¿ytkownika
        /// </summary>
        /// <returns>Dane u¿ytkownika</returns>
        /// <response code="200">Zwrócono dane u¿ytkownika</response>
        /// <response code="401">Nieprawid³owy token lub brak uprawnieñ</response>
        /// <response code="404">Nie znaleziono u¿ytkownika</response>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized(new { Message = "Nieprawid³owy token lub brak uprawnieñ." });
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("Nie znaleziono u¿ytkownika.");
            return Ok(user);
        }

        /// <summary>
        /// Aktualizuje dane zalogowanego u¿ytkownika
        /// </summary>
        /// <param name="dto">Dane do aktualizacji</param>
        /// <returns>Zaktualizowany u¿ytkownik</returns>
        /// <response code="200">U¿ytkownik zosta³ zaktualizowany</response>
        /// <response code="401">Nieprawid³owy token lub brak uprawnieñ</response>
        /// <response code="404">Nie znaleziono u¿ytkownika</response>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserDto dto)
        {
            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized(new { Message = "Nieprawid³owy token lub brak uprawnieñ." });
            }

            var updated = await _userService.UpdateAsync(userId, dto);
            if (updated == null) return NotFound("Nie znaleziono u¿ytkownika.");
            return Ok(updated);
        }

        /// <summary>
        /// Aktualizuje dane wybranego u¿ytkownika
        /// </summary>
        /// <param name="id">ID u¿ytkownika</param>
        /// <param name="dto">Dane do aktualizacji</param>
        /// <returns>Zaktualizowany u¿ytkownik</returns>
        /// <response code="200">U¿ytkownik zosta³ zaktualizowany</response>
        /// <response code="404">Nie znaleziono u¿ytkownika</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Nie znaleziono u¿ytkownika.");
            return Ok(updated);
        }

        /// <summary>
        /// Usuwa konto zalogowanego u¿ytkownika
        /// </summary>
        /// <returns>Brak treœci</returns>
        /// <response code="204">Konto zosta³o usuniête</response>
        /// <response code="400">Nie mo¿na usun¹æ konta administratora</response>
        /// <response code="401">Nieprawid³owy token lub brak uprawnieñ</response>
        /// <response code="404">Nie znaleziono u¿ytkownika</response>
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized(new { Message = "Nieprawid³owy token lub brak uprawnieñ." });
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("Nie znaleziono u¿ytkownika.");

            if (user.Role.ToLower() == "admin")
            {
                return BadRequest(new { Message = "Nie mo¿esz usun¹æ swojego konta, poniewa¿ jesteœ administratorem." });
            }

            var deleted = await _userService.DeleteAsync(userId);
            if (!deleted) return NotFound("Nie uda³o siê usun¹æ u¿ytkownika.");
            return NoContent();
        }

        /// <summary>
        /// Usuwa wybranego u¿ytkownika
        /// </summary>
        /// <param name="id">ID u¿ytkownika</param>
        /// <returns>Brak treœci</returns>
        /// <response code="204">Konto zosta³o usuniête</response>
        /// <response code="400">Nie mo¿na usun¹æ konta administratora</response>
        /// <response code="401">Nieprawid³owy token lub brak uprawnieñ</response>
        /// <response code="404">Nie znaleziono u¿ytkownika</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            int currentUserId;
            try
            {
                currentUserId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized(new { Message = "Nieprawid³owy token lub brak uprawnieñ." });
            }

            if (id == currentUserId)
            {
                return BadRequest(new { Message = "Nie mo¿esz usun¹æ swojego konta, poniewa¿ jesteœ administratorem." });
            }

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound("Nie znaleziono u¿ytkownika.");

            if (user.Role.ToLower() == "admin")
            {
                return BadRequest(new { Message = "Nie mo¿esz usun¹æ konta innego administratora." });
            }

            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound("Nie uda³o siê usun¹æ u¿ytkownika.");
            return NoContent();
        }

        /// <summary>
        /// Zmienia rolê u¿ytkownika
        /// </summary>
        /// <param name="id">ID u¿ytkownika</param>
        /// <param name="role">Nowa rola (user lub admin)</param>
        /// <returns>Brak treœci</returns>
        /// <response code="204">Rola zosta³a zmieniona</response>
        /// <response code="400">Nieprawid³owa rola</response>
        /// <response code="404">Nie znaleziono u¿ytkownika</response>
        [HttpPut("{id}/role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return BadRequest("Rola nie mo¿e byæ pusta.");

            var allowedRoles = new[] { "user", "admin" };
            if (!allowedRoles.Contains(role.ToLower()))
                return BadRequest("Nieprawid³owa rola. Dozwolone: user, admin.");

            var updated = await _userService.ChangeUserRoleAsync(id, role.ToLower());
            if (!updated) return NotFound("Nie znaleziono u¿ytkownika.");
            return NoContent();
        }
    }
}
