using Microsoft.AspNetCore.Mvc;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Quickaid.Utils;
using Quickaid.Models.Entities;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AedController(IAedService aedService, AedGeoJsonUtils geoJsonUtils) : ControllerBase
    {
        private readonly IAedService _aedService = aedService;
        private readonly AedGeoJsonUtils _geoJsonUtils = geoJsonUtils;

        // GET api/aed
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllAedPoints()
        {
            var result = await _aedService.GetMergedAedsAsync();
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
        public async Task<IActionResult> AddAed([FromBody] InternalAedDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _aedService.AddAsync(dto);
            return CreatedAtAction(nameof(GetAedById), new { id = created.Id }, created);
        }

        // PUT api/aed/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAed(int id, [FromBody] InternalAedDto dto)
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

        // GET api/aed/external
        [HttpGet("external")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExternalAeds()
        {;
            try
            {
                var list = await _geoJsonUtils.FetchExternalAedsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Nie uda³o siê pobraæ AED z OpenAEDMap. " + ex);
            }
        }

        // GET api/aed/internal
        [HttpGet("internal")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInternalAeds()
        {
            try
            {
                var list = await _aedService.GetInternalAedsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Nie uda³o siê pobraæ AED z bazy danych. " + ex.Message);
            }
        }


    }
}
