using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;

namespace Quickaid.Services
{
    public class UserService(AppDbContext db) : IUserService
    {
        private readonly AppDbContext _db = db;

        // pobranie wszystkich u¿ytkowników
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _db.Users.ToListAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            });
        }

        // pobranie u¿ytkownika po Id
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        // aktualizacja u¿ytkownika
        public async Task<UserDto?> UpdateAsync(int id, UserDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            // merge pól: jeœli pole w dto jest puste/null, to zachowaj stare
            user.Username = string.IsNullOrWhiteSpace(dto.Username) ? user.Username : dto.Username;
            user.Email = string.IsNullOrWhiteSpace(dto.Email) ? user.Email : dto.Email;

            // role zmienia tylko admin poprzez osobny endpoint
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = dto.Role;
            }

            await _db.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        // usuniêcie u¿ytkownika
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
