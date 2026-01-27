using Microsoft.EntityFrameworkCore;
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
    // Serwis obs³uguj¹cy rejestracjê i logowanie u¿ytkowników
    public class AuthService(AppDbContext db, IConfiguration config) : IAuthService
    {
        private readonly AppDbContext _db = db;
        private readonly IConfiguration _config = config;

        // Rejestruje nowego u¿ytkownika
        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            // Sprawdzenie minimalnej d³ugoœci has³a
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Has³o musi mieæ co najmniej 8 znaków."
                };
            }

            // Sprawdzenie, czy u¿ytkownik ju¿ istnieje
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (existingUser != null)
                return new AuthResultDto { Success = false, Message = "U¿ytkownik ju¿ istnieje" };

            // Utworzenie nowego u¿ytkownika
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = "user",
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Generowanie salt i has³a
            var salt = GenerateSalt();
            var hashed = HashPassword(dto.Password, salt);

            // Zapis has³a do bazy
            var passwordEntity = new Password
            {
                UserId = user.Id,
                Salt = salt,
                HashedPassword = hashed,
                LastChanged = DateTime.UtcNow
            };
            _db.Passwords.Add(passwordEntity);
            await _db.SaveChangesAsync();

            // Generowanie tokenu JWT
            var token = GenerateJwtToken(user);

            return new AuthResultDto
            {
                Success = true,
                Message = "Rejestracja zakoñczona sukcesem",
                Token = token
            };
        }

        // Loguje u¿ytkownika
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            // Wyszukanie u¿ytkownika po nazwie
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return new AuthResultDto { Success = false, Message = "Nieprawid³owa nazwa u¿ytkownika lub has³o" };

            // Wyszukanie has³a w bazie
            var password = await _db.Passwords.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (password == null)
                return new AuthResultDto { Success = false, Message = "Nieprawid³owa nazwa u¿ytkownika lub has³o" };

            // Sprawdzenie poprawnoœci has³a
            var hashedInput = HashPassword(dto.Password, password.Salt ?? "");
            if (hashedInput != password.HashedPassword)
                return new AuthResultDto { Success = false, Message = "Nieprawid³owa nazwa u¿ytkownika lub has³o" };

            // Generowanie tokenu JWT
            var token = GenerateJwtToken(user);

            return new AuthResultDto
            {
                Success = true,
                Message = "Logowanie zakoñczone sukcesem",
                Token = token
            };
        }

        // Generuje losow¹ sól dla has³a
        private static string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[16];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        // Hashuje has³o przy u¿yciu salt
        private static string HashPassword(string password, string salt)
        {
            var combined = Encoding.UTF8.GetBytes(password + salt);
            var hash = SHA256.HashData(combined);
            return Convert.ToBase64String(hash);
        }

        // Generuje token JWT dla u¿ytkownika
        private string GenerateJwtToken(User user)
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
