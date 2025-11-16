using Microsoft.AspNetCore.Mvc;
using Quickaid.Services.Interfaces;
using Quickaid.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // dostêp dla zalogowanych (user + admin)
    public class QuizzesController(IQuizService quizService) : ControllerBase
    {
        // logika aplikacyjna dla quizów
        private readonly IQuizService _quizService = quizService;

        // GET api/quizzes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // pobranie wszystkich quizów
            var quizzes = await _quizService.GetAllAsync();
            return Ok(quizzes);
        }

        // GET api/quizzes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // pobranie quizu po id
            var quiz = await _quizService.GetByIdAsync(id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        // POST api/quizzes
        [Authorize(Roles = "admin")] // tylko administrator dodaje
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] QuizDto dto)
        {
            // walidacja danych wejœciowych
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // dodanie nowego quizu
            var created = await _quizService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/quizzes/{id}
        [Authorize(Roles = "admin")] // tylko admin aktualizuje
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuizDto dto)
        {
            // walidacja danych wejœciowych
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // aktualizacja quizu
            var updated = await _quizService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/quizzes/{id}
        [Authorize(Roles = "admin")] // tylko admin usuwa
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // usuniêcie quizu
            var deleted = await _quizService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
