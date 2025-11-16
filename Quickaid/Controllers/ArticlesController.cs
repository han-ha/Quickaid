using Microsoft.AspNetCore.Mvc;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController(IArticleService articleService) : ControllerBase
    {
        // logika aplikacyjna dla artyku³ów edukacyjnych
        private readonly IArticleService _articleService = articleService;

        // GET api/articles
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // pobranie wszystkich artyku³ów
            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

        // GET api/articles/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // pobranie artyku³u po id
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) return NotFound();
            return Ok(article);
        }

        // POST api/articles
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ArticleDto dto)
        {
            // walidacja danych wejœciowych
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // dodanie artyku³u
            var created = await _articleService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/articles/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleDto dto)
        {
            // walidacja danych wejœciowych
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // aktualizacja artyku³u
            var updated = await _articleService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/articles/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // usuniêcie artyku³u
            var deleted = await _articleService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
