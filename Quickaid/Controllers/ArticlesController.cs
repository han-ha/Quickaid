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

        // GET api/articles
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

        // GET api/articles/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) return NotFound();
            return Ok(article);
        }

        // POST api/articles
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

        // PUT api/articles/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _articleService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/articles/{id}
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
