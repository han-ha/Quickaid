using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    // logika dla quizów i pytañ
    public interface IQuizService
    {
        Task<IEnumerable<QuizDto>> GetAllAsync();
        Task<QuizDto?> GetByIdAsync(int id);
        Task<QuizDto> AddAsync(QuizDto dto);
        Task<QuizDto?> UpdateAsync(int id, QuizDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
