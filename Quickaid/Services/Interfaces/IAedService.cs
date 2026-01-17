using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IAedService
    {
        Task<IEnumerable<InternalAedDto>> GetInternalAedsAsync();
        Task<InternalAedDto?> GetByIdAsync(int id);
        Task<InternalAedDto> AddAsync(InternalAedDto dto);
        Task<InternalAedDto?> UpdateAsync(int id, InternalAedDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<AedDto>> GetMergedAedsAsync();

    }
}
