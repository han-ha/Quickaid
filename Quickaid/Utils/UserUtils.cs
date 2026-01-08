using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Quickaid.Utils
{
    public static class UserUtils
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(idClaim))
                throw new Exception("Nie znaleziono identyfikatora użytkownika w tokenie.");

            return int.Parse(idClaim);
        }
    }
}
