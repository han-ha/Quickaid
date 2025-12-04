using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IAedService
    {
        Task<IEnumerable<AedDto>> GetAllAsync();
        Task<AedDto?> GetByIdAsync(int id);
        Task<AedDto> AddAsync(AedDto dto);
        Task<AedDto?> UpdateAsync(int id, AedDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
