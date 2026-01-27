using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    // Mapuje has³a na encjê Password
    public class PasswordMapper : IPasswordMapper
    {
        // Tworzy encjê Password z has³a i opcjonalnym saltem
        public Password ToEntity(string hashedPassword, int userId, string? salt = null)
        {
            return new Password
            {
                UserId = userId,
                HashedPassword = hashedPassword,
                Salt = salt
            };
        }
    }
}
