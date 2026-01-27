using Microsoft.AspNetCore.Mvc;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController(IArticleService articleService) : ControllerBase
    {
        private readonly IArticleService _articleService = articleService;

        /// <summary>
        /// Pobiera wszystkie artyku³y
        /// </summary>
        /// <returns>Lista artyku³ów</returns>
        /// <response code="200">Zwrócono listê artyku³ów</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas pobierania danych</response>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var articles = await _articleService.GetAllAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania artyku³ów: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera pojedynczy artyku³ po ID
        /// </summary>
        /// <param name="id">ID artyku³u</param>
        /// <returns>Pojedynczy artyku³</returns>
        /// <response code="200">Zwrócono artyku³</response>
        /// <response code="404">Nie znaleziono artyku³u o podanym ID</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas pobierania danych</response>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var article = await _articleService.GetByIdAsync(id);
                if (article == null) return NotFound();
                return Ok(article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania artyku³u: " + ex.Message);
            }
        }

        /// <summary>
        /// Dodaje nowy artyku³
        /// </summary>
        /// <param name="dto">Dane artyku³u</param>
        /// <returns>Utworzony artyku³</returns>
        /// <response code="201">Artyku³ zosta³ utworzony</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        /// <response code="401">Brak uwierzytelnienia</response>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ArticleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("id")?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var created = await _articleService.AddAsync(dto, userId);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Aktualizuje artyku³ o podanym ID
        /// </summary>
        /// <param name="id">ID artyku³u</param>
        /// <param name="dto">Nowe dane artyku³u</param>
        /// <returns>Aktualizowany artyku³</returns>
        /// <response code="200">Artyku³ zosta³ zaktualizowany</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        /// <response code="404">Nie znaleziono artyku³u o podanym ID</response>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _articleService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Usuwa artyku³ po ID
        /// </summary>
        /// <param name="id">ID artyku³u</param>
        /// <response code="204">Artyku³ zosta³ usuniêty</response>
        /// <response code="404">Nie znaleziono artyku³u o podanym ID</response>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _articleService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
