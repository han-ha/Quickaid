using Microsoft.AspNetCore.Mvc;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Quickaid.Utils;
using Quickaid.Enums;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AedController(IAedService aedService, AedGeoJsonUtils geoJsonUtils) : ControllerBase
    {
        private readonly IAedService _aedService = aedService;
        private readonly AedGeoJsonUtils _geoJsonUtils = geoJsonUtils;

        /// <summary>
        /// Pobiera wszystkie punkty AED
        /// </summary>
        /// <returns>Lista punktów AED</returns>
        /// <response code="200">Zwrócono listê AED</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas pobierania danych</response>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllAedPoints()
        {
            try
            {
                var result = await _aedService.GetMergedAedsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania AED: " + ex.Message);
            }
        }


        /// <summary>
        /// Pobiera pojedynczy punkt AED po ID
        /// </summary>
        /// <param name="id">ID punktu AED</param>
        /// <returns>Punkt AED</returns>
        /// <response code="200">Zwrócono AED</response>
        /// <response code="404">Nie znaleziono AED o podanym ID</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas pobierania danych</response>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAedById(int id)
        {
            try
            {
                var point = await _aedService.GetByIdAsync(id);
                if (point == null) return NotFound();
                return Ok(point);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania AED: " + ex.Message);
            }
        }


        /// <summary>
        /// Dodaje nowe AED do bazy danych
        /// </summary>
        /// <param name="dto">Dane AED do dodania</param>
        /// <returns>Utworzony punkt AED</returns>
        /// <response code="201">AED zosta³ utworzony</response>
        /// <response code="400">Niepoprawne dane lub próba dodania AED zewnêtrznego</response>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAed([FromBody] InternalAedDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.Type != AedType.Internal)
                return BadRequest("Mo¿na dodawaæ tylko AED typu Internal");

            var created = await _aedService.AddAsync(dto);
            return CreatedAtAction(nameof(GetAedById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Aktualizuje AED z bazy danych
        /// </summary>
        /// <param name="id">ID punktu AED</param>
        /// <param name="dto">Nowe dane AED</param>
        /// <returns>Aktualizowany punkt AED</returns>
        /// <response code="200">AED zosta³ zaktualizowany</response>
        /// <response code="400">Niepoprawne dane lub próba edycji AED zewnêtrznego</response>
        /// <response code="404">Nie znaleziono AED o podanym ID</response>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAed(int id, [FromBody] InternalAedDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.Type != AedType.Internal)
                return BadRequest("Nie mo¿na edytowaæ AED zewnêtrznych");

            var updated = await _aedService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Usuwa AED po ID
        /// </summary>
        /// <param name="id">ID punktu AED</param>
        /// <returns>Brak treœci</returns>
        /// <response code="204">AED zosta³ usuniêty</response>
        /// <response code="404">Nie znaleziono AED o podanym ID</response>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAed(int id)
        {
            var deleted = await _aedService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Pobiera listê AED zewnêtrznych (OpenAEDMap)
        /// </summary>
        /// <returns>Lista AED zewnêtrznych</returns>
        /// <response code="200">Zwrócono listê AED</response>
        /// <response code="503">B³¹d pobrania danych z OpenAEDMap</response>
        [HttpGet("external")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExternalAeds()
        {
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

        /// <summary>
        /// Pobiera listê AED z bazy danych
        /// </summary>
        /// <returns>Lista AED z bazy danych</returns>
        /// <response code="200">Zwrócono listê AED</response>
        /// <response code="500">B³¹d pobrania danych z bazy</response>
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
