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

        // GET api/answers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var answers = await _answerService.GetAllAsync();
            return Ok(answers);
        }

        // GET api/answers/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var answer = await _answerService.GetByIdAsync(id);
            if (answer == null) return NotFound();
            return Ok(answer);
        }

        // POST api/answers?questionId={questionId}
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add([FromBody] AnswerDto dto, [FromQuery] int questionId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _answerService.AddAsync(dto, questionId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/answers/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] AnswerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _answerService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/answers/{id}
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
