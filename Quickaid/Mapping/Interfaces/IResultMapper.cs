using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera wyników quizów
    public interface IResultMapper
    {
        // Konwertuje encjê Result na DTO
        ResultDto ToDto(Result entity);

        // Konwertuje DTO na encjê Result
        Result ToEntity(ResultDto dto);
    }
}
