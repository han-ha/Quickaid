using Microsoft.AspNetCore.Mvc;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnswersController(IAnswerService answerService) : ControllerBase
    {
        private readonly IAnswerService _answerService = answerService;

        /// <summary>
        /// Pobiera wszystkie odpowiedzi do pytań quizowych
        /// </summary>
        /// <returns>Lista odpowiedzi</returns>
        /// <response code="200">Zwrócono listę odpowiedzi</response>
        /// <response code="500">Wystąpił błąd podczas pobierania danych</response>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var answers = await _answerService.GetAllAsync();
                return Ok(answers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas pobierania odpowiedzi: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera pojedynczą odpowiedź po ID
        /// </summary>
        /// <param name="id">ID odpowiedzi</param>
        /// <returns>Pojedyncza odpowiedź</returns>
        /// <response code="200">Zwrócono odpowiedź</response>
        /// <response code="404">Nie znaleziono odpowiedzi o podanym ID</response>
        /// <response code="500">Wystąpił błąd podczas pobierania danych</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var answer = await _answerService.GetByIdAsync(id);
                if (answer == null) return NotFound();
                return Ok(answer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas pobierania odpowiedzi: " + ex.Message);
            }
        }

        /// <summary>
        /// Dodaje nową odpowiedź do pytania
        /// </summary>
        /// <param name="dto">Dane odpowiedzi</param>
        /// <param name="questionId">ID pytania, do którego dodawana jest odpowiedź</param>
        /// <returns>Utworzona odpowiedź</returns>
        /// <response code="201">Odpowiedź została utworzona</response>
        /// <response code="400">Niepoprawne dane wejściowe</response>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add([FromBody] AnswerDto dto, [FromQuery] int questionId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _answerService.AddAsync(dto, questionId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Aktualizuje istniejącą odpowiedź
        /// </summary>
        /// <param name="id">ID odpowiedzi</param>
        /// <param name="dto">Nowe dane odpowiedzi</param>
        /// <returns>Aktualizowana odpowiedź</returns>
        /// <response code="200">Odpowiedź została zaktualizowana</response>
        /// <response code="400">Niepoprawne dane wejściowe</response>
        /// <response code="404">Nie znaleziono odpowiedzi o podanym ID</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] AnswerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _answerService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Usuwa odpowiedź po ID
        /// </summary>
        /// <param name="id">ID odpowiedzi</param>
        /// <returns>Brak treści</returns>
        /// <response code="204">Odpowiedź została usunięta</response>
        /// <response code="404">Nie znaleziono odpowiedzi o podanym ID</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _answerService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
