using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Mapping.Interfaces;

namespace Quickaid.Mapping
{
    public class UserMapper : IUserMapper
    {
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

