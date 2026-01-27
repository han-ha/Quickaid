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

        /// <summary>
        /// Pobiera wszystkie wyniki
        /// </summary>
        /// <returns>Lista wyników</returns>
        /// <response code="200">Zwrócono listê wyników</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas pobierania danych</response>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var results = await _resultService.GetAllAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas pobierania wyników: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera pojedynczy wynik po ID
        /// </summary>
        /// <param name="id">ID wyniku</param>
        /// <returns>Pojedynczy wynik</returns>
        /// <response code="200">Zwrócono wynik</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="403">Brak dostêpu do danego wyniku</response>
        /// <response code="404">Nie znaleziono wyniku o podanym ID</response>
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
                return Unauthorized("Nieprawid³owy token u¿ytkownika.");
            }

            var result = await _resultService.GetByIdAsync(id);
            if (result == null) return NotFound();

            if (!User.IsInRole("admin") && result.UserId != userId)
                return Forbid();

            return Ok(result);
        }

        /// <summary>
        /// Pobiera wyniki konkretnego u¿ytkownika
        /// </summary>
        /// <param name="userId">ID u¿ytkownika</param>
        /// <returns>Lista wyników u¿ytkownika</returns>
        /// <response code="200">Zwrócono listê wyników</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="403">Brak dostêpu do wyników danego u¿ytkownika</response>
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
                return Unauthorized("Nieprawid³owy token u¿ytkownika.");
            }

            if (!User.IsInRole("admin") && userId != currentUserId)
                return Forbid();

            var results = await _resultService.GetByUserAsync(userId);
            return Ok(results);
        }

        /// <summary>
        /// Dodaje nowy wynik
        /// </summary>
        /// <param name="dto">Dane wyniku</param>
        /// <returns>Utworzony wynik</returns>
        /// <response code="200">Wynik zosta³ dodany</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas dodawania wyniku</response>
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
                    return Unauthorized("Nieprawid³owy token u¿ytkownika.");
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
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas dodawania wyniku: " + ex.Message);
            }
        }

        /// <summary>
        /// Aktualizuje wynik po ID
        /// </summary>
        /// <param name="id">ID wyniku</param>
        /// <param name="dto">Nowe dane wyniku</param>
        /// <returns>Aktualizowany wynik</returns>
        /// <response code="200">Wynik zosta³ zaktualizowany</response>
        /// <response code="400">Niepoprawne dane wejœciowe</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="403">Brak dostêpu do wyniku danego u¿ytkownika</response>
        /// <response code="404">Nie znaleziono wyniku o podanym ID</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas aktualizacji wyniku</response>
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
                    return Unauthorized("Nieprawid³owy token u¿ytkownika.");
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
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas aktualizacji wyniku: " + ex.Message);
            }
        }

        /// <summary>
        /// Usuwa wynik po ID
        /// </summary>
        /// <param name="id">ID wyniku</param>
        /// <returns>Brak treœci</returns>
        /// <response code="204">Wynik zosta³ usuniêty</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="403">Brak dostêpu do wyniku danego u¿ytkownika</response>
        /// <response code="404">Nie znaleziono wyniku o podanym ID</response>
        /// <response code="500">Wyst¹pi³ b³¹d podczas usuwania wyniku</response>
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
                return Unauthorized("Nieprawid³owy token u¿ytkownika.");
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
                return StatusCode(500, "Wyst¹pi³ b³¹d podczas usuwania wyniku: " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera najlepszy wynik u¿ytkownika dla konkretnego quizu
        /// </summary>
        /// <param name="quizId">ID quizu</param>
        /// <returns>Najlepszy wynik u¿ytkownika</returns>
        /// <response code="200">Zwrócono wynik</response>
        /// <response code="401">Nieprawid³owy token u¿ytkownika</response>
        /// <response code="404">Nie znaleziono wyniku</response>
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
                return Unauthorized("Nieprawid³owy token u¿ytkownika.");
            }

            var bestResult = await _resultService.GetBestResultForUserAsync(userId, quizId);
            if (bestResult == null) return NotFound();

            return Ok(bestResult);
        }
    }
}
