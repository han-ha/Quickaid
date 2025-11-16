using Microsoft.AspNetCore.Mvc;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize(Roles = "admin")] // tylko admin widzi wszystkich
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET api/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")] // tylko admin widzi dowolnego
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // GET api/users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { Message = "Nieprawid³owy token lub brak uprawnieñ." });

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(user);
        }

        // PUT api/users/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { Message = "Nieprawid³owy token lub brak uprawnieñ." });

            var updated = await _userService.UpdateAsync(userId, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // PUT api/users/{id} (admin aktualizuje dowolnego u¿ytkownika)
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE api/users/me
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { Message = "Nieprawid³owy token lub brak uprawnieñ." });

            var deleted = await _userService.DeleteAsync(userId);
            if (!deleted) return NotFound();

            return NoContent();
        }

        // DELETE api/users/{id} (admin usuwa dowolnego)
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
