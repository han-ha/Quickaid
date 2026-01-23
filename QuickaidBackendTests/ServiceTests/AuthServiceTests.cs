using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services;
using Microsoft.Extensions.Configuration;
using QuickaidBackendTests.TestHelpers;
using System.Text;
using System.Security.Cryptography;

namespace QuickaidBackendTests.ServiceTests
{
    [TestClass]
    public class AuthServiceTests
    {
        private AppDbContext _db = null!;
        private AuthService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();

            var settings = new Dictionary<string, string>
            {
                { "Jwt:Key", "THIS_IS_A_SUPER_SECRET_TEST_KEY_123456" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpireHours", "24" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _service = new AuthService(_db, config);
        }

        [TestMethod]
        // Test RegisterAsync - poprawna rejestracja nowego użytkownika
        public async Task RegisterAsync_ValidUser_ReturnsSuccess()
        {
            var dto = new RegisterDto
            {
                Username = "newuser",
                Email = "new@test.com",
                Password = "StrongPass1"
            };

            var result = await _service.RegisterAsync(dto);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Token);
            Assert.AreEqual("Rejestracja zakończona sukcesem", result.Message);

            var userInDb = await _db.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
            Assert.IsNotNull(userInDb);
        }

        [TestMethod]
        // Test RegisterAsync - jeśli hasło jest za krótkie, to rejestracja kończy się niepowodzeniem
        public async Task RegisterAsync_ShortPassword_ReturnsFailure()
        {
            var dto = new RegisterDto
            {
                Username = "shortpass",
                Email = "short@test.com",
                Password = "123"
            };

            var result = await _service.RegisterAsync(dto);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Hasło musi mieć co najmniej 8 znaków.", result.Message);
        }

        [TestMethod]
        // Test RegisterAsync - próba rejestracji istniejącego użytkownika kończy się niepowodzeniem
        public async Task RegisterAsync_ExistingUser_ReturnsFailure()
        {
            _db.Users.Add(new User { Username = "exist", Email = "exist@test.com" });
            await _db.SaveChangesAsync();

            var dto = new RegisterDto
            {
                Username = "exist",
                Email = "exist@test.com",
                Password = "StrongPass1"
            };

            var result = await _service.RegisterAsync(dto);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Użytkownik już istnieje", result.Message);
        }

        [TestMethod]
        // Test LoginAsync - poprawne logowanie użytkownika z prawidłowym hasłem
        public async Task LoginAsync_ValidUser_ReturnsSuccess()
        {
            var user = new User { Username = "loginuser", Email = "login@test.com" };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var salt = "somesalt";
            var hash = Convert.ToBase64String(SHA256.HashData(
                Encoding.UTF8.GetBytes("StrongPass1" + salt)));

            _db.Passwords.Add(new Password { UserId = user.Id, Salt = salt, HashedPassword = hash });
            await _db.SaveChangesAsync();

            var dto = new LoginDto { Username = "loginuser", Password = "StrongPass1" };
            var result = await _service.LoginAsync(dto);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Token);
            Assert.AreEqual("Logowanie zakończone sukcesem", result.Message);
        }

        [TestMethod]
        // Test LoginAsync - nieprawidłowe hasło kończy się niepowodzeniem
        public async Task LoginAsync_WrongPassword_ReturnsFailure()
        {
            var user = new User { Username = "user1", Email = "u1@test.com" };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var salt = "salt1";
            var hash = Convert.ToBase64String(SHA256.HashData(
                Encoding.UTF8.GetBytes("correct" + salt)));

            _db.Passwords.Add(new Password { UserId = user.Id, Salt = salt, HashedPassword = hash });
            await _db.SaveChangesAsync();

            var dto = new LoginDto { Username = "user1", Password = "wrongpass" };
            var result = await _service.LoginAsync(dto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Nieprawidłowa nazwa użytkownika lub hasło", result.Message);
        }

        [TestMethod]
        // Test LoginAsync - logowanie nieistniejącego użytkownika kończy się niepowodzeniem
        public async Task LoginAsync_NonExistingUser_ReturnsFailure()
        {
            var dto = new LoginDto { Username = "nosuch", Password = "whatever" };
            var result = await _service.LoginAsync(dto);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Nieprawidłowa nazwa użytkownika lub hasło", result.Message);
        }
    }
}
