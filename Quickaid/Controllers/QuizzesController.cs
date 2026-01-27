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
    public class QuizzesController(
        IQuizService quizService,
        IQuizSolverService quizSolverService,
        IQuestionService questionService) : ControllerBase
    {
        private readonly IQuizService _quizService = quizService;
        private readonly IQuizSolverService _quizSolverService = quizSolverService;
        private readonly IQuestionService _questionService = questionService;

        /// <summary>
        /// Pobiera wszystkie quizy
        /// </summary>
        /// <returns>Lista quizów</returns>
        /// <response code="200">Zwrócono listê quizów</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas pobierania danych</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var quizzes = await _quizService.GetAllAsync();
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania quizów: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera pojedynczy quiz po ID
        /// </summary>
        /// <param name="id">ID quizu</param>
        /// <returns>Pojedynczy quiz</returns>
        /// <response code="200">Zwrócono quiz</response>
        /// <response code="404">Nie znaleziono quizu o podanym ID</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas pobierania danych</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var quiz = await _quizService.GetByIdAsync(id);
                if (quiz == null) return NotFound();
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania quizu: " + ex.Message);
            }
        }

        /// <summary>
        /// Tworzy nowy quiz
        /// </summary>
        /// <param name="dto">Dane quizu</param>
        /// <returns>Utworzony quiz</returns>
        /// <response code="201">Quiz zosta³ utworzony</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] QuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _quizService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Aktualizuje istniej¹cy quiz
        /// </summary>
        /// <param name="id">ID quizu</param>
        /// <param name="dto">Nowe dane quizu</param>
        /// <returns>Aktualizowany quiz</returns>
        /// <response code="200">Quiz zosta³ zaktualizowany</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        /// <response code="404">Nie znaleziono quizu o podanym ID</response>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _quizService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Usuwa quiz po ID
        /// </summary>
        /// <param name="id">ID quizu</param>
        /// <returns>Brak treœci</returns>
        /// <response code="204">Quiz zosta³ usuniêty</response>
        /// <response code="404">Nie znaleziono quizu o podanym ID</response>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _quizService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Zg³asza odpowiedzi u¿ytkownika do quizu
        /// </summary>
        /// <param name="quizId">ID quizu</param>
        /// <param name="submission">Dane odpowiedzi u¿ytkownika</param>
        /// <returns>Wynik quizu</returns>
        /// <response code="200">Zwrócono wynik quizu</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="404">Nie znaleziono quizu</response>
        [HttpPost("{quizId}/submit")]
        public async Task<IActionResult> SubmitQuiz(int quizId, [FromBody] QuizSubmissionDto submission)
        {
            if (submission == null)
                return BadRequest("Treœæ submitu nie mo¿e byæ pusta.");

            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized("Nieprawid³owy token u¿ytkownika.");
            }

            var quiz = await _quizService.GetByIdAsync(quizId);
            if (quiz == null)
                return NotFound("Nie znaleziono quizu.");

            var quizQuestionIds = await _quizService.GetQuestionsIdsAsync(quizId);

            if (submission.Answers.Keys.Except(quizQuestionIds).Any())
                return BadRequest("Niektóre przes³ane pytania nie nale¿¹ do tego quizu.");

            if (submission.Answers.Count > quizQuestionIds.Count)
                return BadRequest("Przes³ano zbyt wiele odpowiedzi.");

            var result = await _quizSolverService.SubmitQuizAsync(userId, quizId, submission.Answers);

            return Ok(result);
        }

        /// <summary>
        /// Pobiera ostatni wynik zalogowanego u¿ytkownika dla quizu
        /// </summary>
        /// <param name="quizId">ID quizu</param>
        /// <returns>Ostatni wynik u¿ytkownika</returns>
        /// <response code="200">Zwrócono wynik</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="404">Nie znaleziono wyniku</response>
        [HttpGet("{quizId}/results/me")]
        public async Task<IActionResult> GetMyLastResult(int quizId)
        {
            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized("Nieprawid³owy token u¿ytkownika.");
            }

            var lastResult = await _quizSolverService.GetLastResultAsync(userId, quizId);
            if (lastResult == null)
                return NotFound("Nie znaleziono wyników dla tego quizu.");

            return Ok(lastResult);
        }

        /// <summary>
        /// Dodaje pytanie do istniej¹cego quizu
        /// </summary>
        /// <param name="quizId">ID quizu</param>
        /// <param name="dto">Dane pytania</param>
        /// <returns>Utworzone pytanie</returns>
        /// <response code="201">Pytanie zosta³o dodane do quizu</response>
        /// <response code="400">Niepoprawne dane wejœciowe lub b³¹d podczas dodawania</response>
        [Authorize(Roles = "admin")]
        [HttpPost("{quizId}/questions")]
        public async Task<IActionResult> AddQuestionToQuiz(int quizId, [FromBody] QuestionDto dto)
        {
            try
            {
                var created = await _questionService.AddToQuizAsync(quizId, dto);
                return CreatedAtAction(
                    nameof(QuestionsController.GetById),
                    "Questions",
                    new { id = created.Id },
                    created
                );
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
