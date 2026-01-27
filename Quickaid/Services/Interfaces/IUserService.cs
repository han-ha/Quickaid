using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // Interfejs serwisu do obs³ugi u¿ytkowników
    public interface IUserService
    {
        // Zwraca wszystkich u¿ytkowników
        Task<IEnumerable<UserDto>> GetAllAsync();

        // Zwraca u¿ytkownika po Id
        Task<UserDto?> GetByIdAsync(int id);

        // Aktualizuje u¿ytkownika o podanym Id
        Task<UserDto?> UpdateAsync(int id, UserDto dto);

        // Usuwa u¿ytkownika po Id
        Task<bool> DeleteAsync(int id);

        // Zmienia rolê u¿ytkownika
        Task<bool> ChangeUserRoleAsync(int userId, string newRole);
    }
}
