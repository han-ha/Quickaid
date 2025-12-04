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

        // GET api/users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET api/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound("Nie znaleziono u¿ytkownika.");
            return Ok(user);
        }

        // GET api/users/me
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

        // PUT api/users/me
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

        // PUT api/users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Nie znaleziono u¿ytkownika.");
            return Ok(updated);
        }

        // DELETE api/users/me
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

            var deleted = await _userService.DeleteAsync(userId);
            if (!deleted) return NotFound("Nie znaleziono u¿ytkownika.");
            return NoContent();
        }

        // DELETE api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound("Nie znaleziono u¿ytkownika.");
            return NoContent();
        }

        // PUT api/users/{id}/role
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
