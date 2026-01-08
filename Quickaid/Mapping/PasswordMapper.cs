using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class PasswordMapper : IPasswordMapper
    {
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

