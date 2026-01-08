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
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        // GET api/results
        [Authorize(Roles = "admin")]
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
            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized("Nieprawid這wy token u篡tkownika.");
            }

            var result = await _resultService.GetByIdAsync(id);
            if (result == null) return NotFound();

            if (!User.IsInRole("admin") && result.UserId != userId)
                return Forbid();

            return Ok(result);
        }

        // GET api/results/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserResults(int userId)
        {
            int currentUserId;
            try
            {
                currentUserId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized("Nieprawid這wy token u篡tkownika.");
            }

            if (!User.IsInRole("admin") && userId != currentUserId)
                return Forbid();

            var results = await _resultService.GetByUserAsync(userId);
            return Ok(results);
        }

        // POST api/results
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ResultDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!User.IsInRole("admin"))
            {
                int userId;
                try
                {
                    userId = UserUtils.GetUserId(User);
                }
                catch
                {
                    return Unauthorized("Nieprawid這wy token u篡tkownika.");
                }

                dto.UserId = userId;
            }

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

            if (!User.IsInRole("admin"))
            {
                int userId;
                try
                {
                    userId = UserUtils.GetUserId(User);
                }
                catch
                {
                    return Unauthorized("Nieprawid這wy token u篡tkownika.");
                }

                if (dto.UserId != userId)
                    return Forbid();
            }

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
            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized("Nieprawid這wy token u篡tkownika.");
            }

            var result = await _resultService.GetByIdAsync(id);
            if (result == null) return NotFound();

            if (!User.IsInRole("admin") && result.UserId != userId)
                return Forbid();

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

        // GET api/results/best/{quizId}
        [HttpGet("best/{quizId}")]
        public async Task<IActionResult> GetBestForQuiz(int quizId)
        {
            int userId;
            try
            {
                userId = UserUtils.GetUserId(User);
            }
            catch
            {
                return Unauthorized("Nieprawid這wy token u篡tkownika.");
            }

            var bestResult = await _resultService.GetBestResultForUserAsync(userId, quizId);
            if (bestResult == null) return NotFound();

            return Ok(bestResult);
        }

    }
}
