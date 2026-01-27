using Microsoft.AspNetCore.Mvc;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        /// <summary>
        /// Pobiera wszystkie pytania
        /// </summary>
        /// <returns>Lista pytań</returns>
        /// <response code="200">Zwrócono listę pytań</response>
        /// <response code="500">Wystąpił błąd podczas pobierania danych</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _questionService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas pobierania pytań: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera pojedyncze pytanie po ID
        /// </summary>
        /// <param name="id">ID pytania</param>
        /// <returns>Pojedyncze pytanie</returns>
        /// <response code="200">Zwrócono pytanie</response>
        /// <response code="404">Nie znaleziono pytania o podanym ID</response>
        /// <response code="500">Wystąpił błąd podczas pobierania danych</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id);
                if (question == null) return NotFound();
                return Ok(question);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas pobierania pytania: " + ex.Message);
            }
        }

        /// <summary>
        /// Dodaje nowe pytanie
        /// </summary>
        /// <param name="dto">Dane pytania</param>
        /// <returns>Utworzone pytanie</returns>
        /// <response code="201">Pytanie zostało utworzone</response>
        /// <response code="400">Niepoprawne dane wejściowe</response>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] QuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _questionService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Aktualizuje istniejące pytanie
        /// </summary>
        /// <param name="id">ID pytania</param>
        /// <param name="dto">Nowe dane pytania</param>
        /// <returns>Aktualizowane pytanie</returns>
        /// <response code="200">Pytanie zostało zaktualizowane</response>
        /// <response code="400">Niepoprawne dane wejściowe</response>
        /// <response code="404">Nie znaleziono pytania o podanym ID</response>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _questionService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Usuwa powiązanie pytania z quizem
        /// </summary>
        /// <param name="questionId">ID pytania</param>
        /// <param name="quizId">ID quizu</param>
        /// <response code="204">Powiązanie zostało usunięte</response>
        /// <response code="404">Nie znaleziono pytania lub powiązania z quizem</response>
        /// <response code="400">Błąd podczas usuwania powiązania</response>
        [Authorize(Roles = "admin")]
        [HttpDelete("{questionId}/quiz/{quizId}")]
        public async Task<IActionResult> Delete(int questionId, int quizId)
        {
            try
            {
                var deleted = await _questionService.DeleteAsync(questionId, quizId);
                if (!deleted)
                    return NotFound("Nie znaleziono pytania lub powiązania z quizem.");
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Dodaje pytanie do istniejącego quizu
        /// </summary>
        /// <param name="quizId">ID quizu</param>
        /// <param name="dto">Dane pytania</param>
        /// <returns>Utworzone pytanie</returns>
        /// <response code="201">Pytanie zostało dodane do quizu</response>
        /// <response code="400">Niepoprawne dane wejściowe lub błąd podczas dodawania</response>
        [Authorize(Roles = "admin")]
        [HttpPost("quiz/{quizId}")]
        public async Task<IActionResult> AddQuestionToQuiz(int quizId, [FromBody] QuestionDto dto)
        {
            if (dto == null) return BadRequest("Pytanie nie może być puste.");

            try
            {
                var created = await _questionService.AddToQuizAsync(quizId, dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
