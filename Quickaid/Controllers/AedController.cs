using Microsoft.AspNetCore.Mvc;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AedController(IAedService aedService) : ControllerBase
    {
        private readonly IAedService _aedService = aedService;

        // GET api/aed
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllAedPoints()
        {
            var result = await _aedService.GetAllAsync();
            return Ok(result);
        }

        // GET api/aed/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAedById(int id)
        {
            var point = await _aedService.GetByIdAsync(id);
            if (point == null) return NotFound();
            return Ok(point);
        }

        // POST api/aed
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAed([FromBody] AedDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _aedService.AddAsync(dto);
            return CreatedAtAction(nameof(GetAedById), new { id = created.Id }, created);
        }

        // PUT api/aed/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAed(int id, [FromBody] AedDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _aedService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/aed/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAed(int id)
        {
            var deleted = await _aedService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
