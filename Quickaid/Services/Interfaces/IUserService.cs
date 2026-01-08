using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> UpdateAsync(int id, UserDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeUserRoleAsync(int userId, string newRole);
    }
}
