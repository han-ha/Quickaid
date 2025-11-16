using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IResultService
    {
        Task<IEnumerable<ResultDto>> GetAllAsync();
        Task<IEnumerable<ResultDto>> GetByUserAsync(int userId);
        Task<ResultDto> AddAsync(ResultDto dto);
        Task<ResultDto?> GetByIdAsync(int id);
        Task<ResultDto?> UpdateAsync(int id, ResultDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
