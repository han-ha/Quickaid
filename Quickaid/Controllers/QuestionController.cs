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

        // GET api/questions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _questionService.GetAllAsync();
            return Ok(result);
        }

        // GET api/questions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        // POST api/questions
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] QuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _questionService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/questions/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _questionService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/questions/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _questionService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
