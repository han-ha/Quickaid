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
    public class QuizzesController(IQuizService quizService, IQuizSolverService quizSolverService) : ControllerBase
    {
        private readonly IQuizService _quizService = quizService;
        private readonly IQuizSolverService _quizSolverService = quizSolverService;

        // GET api/quizzes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quizzes = await _quizService.GetAllAsync();
            return Ok(quizzes);
        }

        // GET api/quizzes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _quizService.GetByIdAsync(id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        // POST api/quizzes
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] QuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _quizService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/quizzes/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _quizService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/quizzes/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _quizService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

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

        // GET api/quizzes/{quizId}/results/me
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


    }
}
