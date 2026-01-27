using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Quickaid.Utils
{
    // Klasa pomocnicza do pracy z danymi użytkownika z tokena/ClaimsPrincipal
    public static class UserUtils
    {
        // Zwraca Id użytkownika z tokena JWT (ClaimsPrincipal)
        // Rzuca wyjątek, jeśli identyfikator nie został znaleziony
        public static int GetUserId(ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                throw new Exception("Nie znaleziono identyfikatora użytkownika w tokenie.");

            return int.Parse(idClaim);
        }
    }
}
