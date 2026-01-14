using Quickaid.Models.DTO;

namespace Quickaid.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDto>> GetAllAsync();
        Task<QuestionDto?> GetByIdAsync(int id);
        Task<QuestionDto> AddAsync(QuestionDto dto);
        Task<QuestionDto?> UpdateAsync(int id, QuestionDto dto);
        Task<bool> DeleteAsync(int questionId, int quizId);
        Task<List<QuestionDto>> GetByQuizIdAsync(int quizId);
        Task<QuestionDto> AddToQuizAsync(int quizId, QuestionDto dto);


    }
}
