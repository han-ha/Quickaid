using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // logika zarz¹dzania u¿ytkownikami
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> UpdateAsync(int id, UserDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
