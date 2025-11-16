using Microsoft.AspNetCore.Mvc;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // dostêp tylko dla zalogowanych
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        // GET api/results
        [Authorize(Roles = "admin")] // tylko admin widzi wszystkie wyniki
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _resultService.GetAllAsync();
            return Ok(results);
        }

        // GET api/results/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _resultService.GetByIdAsync(id);
            if (result == null) return NotFound();

            if (!User.IsInRole("admin") && result.UserId != GetCurrentUserId())
                return Forbid(); // zwyk³y user nie mo¿e widzieæ cudzych wyników

            return Ok(result);
        }

        // GET api/results/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserResults(int userId)
        {
            if (!User.IsInRole("admin") && userId != GetCurrentUserId())
                return Forbid(); // zwyk³y user mo¿e widzieæ tylko swoje wyniki

            var results = await _resultService.GetByUserAsync(userId);
            return Ok(results);
        }

        // POST api/results
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ResultDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // user mo¿e dodawaæ tylko swoje wyniki
            if (!User.IsInRole("admin"))
                dto.UserId = GetCurrentUserId();

            try
            {
                var created = await _resultService.AddAsync(dto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/results/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResultDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // zwyk³y user mo¿e aktualizowaæ tylko swoje wyniki
            if (!User.IsInRole("admin") && dto.UserId != GetCurrentUserId())
                return Forbid();

            try
            {
                var updated = await _resultService.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // DELETE api/results/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _resultService.GetByIdAsync(id);
            if (result == null) return NotFound();

            if (!User.IsInRole("admin") && result.UserId != GetCurrentUserId())
                return Forbid(); // zwyk³y user nie mo¿e usuwaæ cudzych wyników

            try
            {
                var deleted = await _resultService.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // pomocnicza metoda do pobrania ID aktualnie zalogowanego u¿ytkownika z JWT
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
