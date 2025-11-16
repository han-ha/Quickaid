using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Quickaid.Services
{
    public class AuthService(AppDbContext db, IConfiguration config) : IAuthService
    {
        private readonly AppDbContext _db = db;
        private readonly IConfiguration _config = config;

        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            // sprawdzenie czy u¿ytkownik ju¿ istnieje
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (existingUser != null)
                return new AuthResult { Success = false, Message = "U¿ytkownik ju¿ istnieje" };

            // utworzenie nowego u¿ytkownika
            var user = new Models.Entities.User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = "user",
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // zapisanie has³a
            var salt = GenerateSalt();
            var hashed = HashPassword(dto.Password, salt);

            var passwordEntity = new Password
            {
                UserId = user.Id,
                Salt = salt,
                HashedPassword = hashed,
                LastChanged = DateTime.UtcNow
            };
            _db.Passwords.Add(passwordEntity);
            await _db.SaveChangesAsync();

            // utwórz token JWT
            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Message = "Rejestracja zakoñczona sukcesem",
                Token = token
            };
        }

        public async Task<AuthResult> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return new AuthResult { Success = false, Message = "Nieprawid³owa nazwa u¿ytkownika lub has³o" };

            var password = await _db.Passwords.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (password == null)
                return new AuthResult { Success = false, Message = "Nieprawid³owa nazwa u¿ytkownika lub has³o" };

            var hashedInput = HashPassword(dto.Password, password.Salt ?? "");
            if (hashedInput != password.HashedPassword)
                return new AuthResult { Success = false, Message = "Nieprawid³owa nazwa u¿ytkownika lub has³o" };

            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Message = "Logowanie zakoñczone sukcesem",
                Token = token
            };
        }

        private static string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[16];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string HashPassword(string password, string salt)
        {
            var combined = Encoding.UTF8.GetBytes(password + salt);
            var hash = SHA256.HashData(combined);
            return Convert.ToBase64String(hash);
        }

        private string GenerateJwtToken(Models.Entities.User user)
        {
            var key = _config["Jwt:Key"] ?? throw new Exception("Brak klucza JWT w konfiguracji");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireHours = int.Parse(_config["Jwt:ExpireHours"] ?? "24");

            var keyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(keyBytes, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Username),
                new("id", user.Id.ToString()),
                new(ClaimTypes.Role, user.Role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
