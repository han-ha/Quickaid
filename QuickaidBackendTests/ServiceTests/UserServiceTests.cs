using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using Quickaid.Services;
using QuickaidBackendTests.TestHelpers;
using System.Linq;
using System.Threading.Tasks;

namespace QuickaidBackendTests.ServiceTests;

[TestClass]
public class UserServiceTests
{
    private AppDbContext _db = null!;
    private UserService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = DbHelper.CreateInMemoryDb();
        DbHelper.SeedUsers(_db);

        _service = new UserService(_db);
    }

    [TestMethod]
    // Test GetAllAsync - zwraca wszystkich użytkowników z bazy
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var result = await _service.GetAllAsync();
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    // Test GetByIdAsync - zwraca użytkownika dla istniejącego Id
    public async Task GetByIdAsync_UserExists_ReturnsUser()
    {
        var user = await _service.GetByIdAsync(1);
        Assert.IsNotNull(user);
        Assert.AreEqual("admin", user.Role);
    }

    [TestMethod]
    // Test GetByIdAsync - zwraca null, gdy użytkownik nie istnieje
    public async Task GetByIdAsync_UserNotExists_ReturnsNull()
    {
        var user = await _service.GetByIdAsync(99);
        Assert.IsNull(user);
    }

    [TestMethod]
    // Test UpdateAsync - aktualizuje username i email istniejącego użytkownika
    public async Task UpdateAsync_UpdatesUsernameAndEmail()
    {
        var dto = new UserDto { Username = "new", Email = "new@test.com" };
        var result = await _service.UpdateAsync(2, dto);
        Assert.IsNotNull(result);
        Assert.AreEqual("new", result.Username);
        Assert.AreEqual("new@test.com", result.Email);
    }

    [TestMethod]
    // Test DeleteAsync - usuwa istniejącego użytkownika i zwraca true
    public async Task DeleteAsync_UserExists_ReturnsTrue()
    {
        var result = await _service.DeleteAsync(2);
        Assert.IsTrue(result);
        Assert.IsNull(await _db.Users.FindAsync(2));
    }

    [TestMethod]
    // Test ChangeUserRoleAsync - zmienia rolę użytkownika na nową
    public async Task ChangeUserRoleAsync_UserExists_UpdatesRole()
    {
        var result = await _service.ChangeUserRoleAsync(2, "admin");
        Assert.IsTrue(result);
        Assert.AreEqual("admin", _db.Users.First(u => u.Id == 2).Role);
    }
}
