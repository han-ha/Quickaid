using Microsoft.AspNetCore.Mvc;
using Quickaid.Data;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController(AppDbContext db) : ControllerBase
    {
        /// <summary>
        /// Sprawdza działanie backendu.
        /// </summary>
        /// <returns>Komunikat powitalny</returns>
        /// <response code="200">Backend działa poprawnie</response>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World, backend here!");
        }
    }
}
