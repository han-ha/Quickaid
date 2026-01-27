using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    // Interfejs mapera u¿ytkowników
    public interface IUserMapper
    {
        // Konwertuje encjê User na DTO
        UserDto ToDto(User entity);

        // Konwertuje DTO na encjê User
        User ToEntity(UserDto dto);
    }
}
