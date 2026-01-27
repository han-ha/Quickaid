using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    // Mapuje u¿ytkowników miêdzy encj¹ a DTO
    public class UserMapper : IUserMapper
    {
        // Konwertuje encjê User na DTO
        public UserDto ToDto(User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                Role = entity.Role
            };
        }

        // Konwertuje DTO na encjê User
        public User ToEntity(UserDto dto)
        {
            return new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = dto.Role
            };
        }
    }
}
