using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera hase³
    public interface IPasswordMapper
    {
        // Tworzy encjê Password z has³a i opcjonalnego saltu
        Password ToEntity(string hashedPassword, int userId, string? salt = null);
    }
}
