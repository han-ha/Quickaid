using Quickaid.Models.DTO;
using Quickaid.Models.Entities;

namespace Quickaid.Mapping.Interfaces
{
    public interface IUserMapper
    {
        UserDto ToDto(User entity);
        User ToEntity(UserDto dto);
    }
}

