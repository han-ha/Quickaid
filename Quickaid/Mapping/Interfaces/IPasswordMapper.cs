using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IPasswordMapper
    {
        Password ToEntity(string hashedPassword, int userId, string? salt = null);
    }
}

