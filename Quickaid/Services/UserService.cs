using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;

namespace Quickaid.Services
{
    // Serwis obs³uguj¹cy u¿ytkowników i ich role
    public class UserService(AppDbContext db) : IUserService
    {
        private readonly AppDbContext _db = db;

        // Zwraca wszystkich u¿ytkowników
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

        // Zwraca u¿ytkownika po Id
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

        // Aktualizuje dane u¿ytkownika
        public async Task<UserDto?> UpdateAsync(int id, UserDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            // Aktualizacja pól tylko jeœli podano wartoœci
            user.Username = string.IsNullOrWhiteSpace(dto.Username) ? user.Username : dto.Username;
            user.Email = string.IsNullOrWhiteSpace(dto.Email) ? user.Email : dto.Email;

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

        // Usuwa u¿ytkownika z bazy
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        // Zmienia rolê u¿ytkownika
        public async Task<bool> ChangeUserRoleAsync(int userId, string newRole)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            user.Role = newRole;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
