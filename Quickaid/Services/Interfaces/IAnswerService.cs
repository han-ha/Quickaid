using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IAnswerService
    {
        Task<IEnumerable<AnswerDto>> GetAllAsync();
        Task<AnswerDto?> GetByIdAsync(int id);
        Task<AnswerDto> AddAsync(AnswerDto dto, int questionId);
        Task<AnswerDto?> UpdateAsync(int id, AnswerDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
