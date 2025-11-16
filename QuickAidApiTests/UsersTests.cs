using System.Net.Http.Json;
using Quickaid.Models.DTO;
namespace QuickaidApiTests;

public static class UsersTests
{
    public static async Task Run(HttpClient client)
    {
        Console.WriteLine("=== Users Tests ===");

        // GET all users
        var users = await client.GetFromJsonAsync<List<UserDto>>("api/users");
        Console.WriteLine($"GET /api/users -> {users?.Count} użytkowników");

        // Możesz dodać inne testy jeśli endpointy będą dodawać użytkowników
        Console.WriteLine();
    }
}
