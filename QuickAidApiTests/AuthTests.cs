using System.Net.Http.Json;
using Quickaid.Models.DTO;

namespace QuickaidApiTests;

public static class AuthTests
{
    public static async Task Run(HttpClient client)
    {
        Console.WriteLine("=== Auth Tests ===");

        // generujemy unikalne dane dla testu rejestracji
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
        var newUser = new RegisterDto
        {
            Username = $"testuser_{uniqueSuffix}",
            Email = $"test{uniqueSuffix}@example.com",
            Password = "Test123!"
        };

        // POST /api/auth/register
        var registerResponse = await client.PostAsJsonAsync("api/auth/register", newUser);
        if (registerResponse.IsSuccessStatusCode)
        {
            var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResult>();
            Console.WriteLine($"POST /api/auth/register -> OK, token={authResult?.Token}");
        }
        else
        {
            Console.WriteLine($"POST /api/auth/register -> {registerResponse.StatusCode}");
            Console.WriteLine("Treść odpowiedzi: " + await registerResponse.Content.ReadAsStringAsync());
        }

        // POST /api/auth/login
        var loginDto = new LoginDto
        {
            Username = newUser.Username,
            Password = newUser.Password
        };

        var loginResponse = await client.PostAsJsonAsync("api/auth/login", loginDto);
        if (loginResponse.IsSuccessStatusCode)
        {
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResult>();
            Console.WriteLine($"POST /api/auth/login -> OK, token={loginResult?.Token}");
        }
        else
        {
            Console.WriteLine($"POST /api/auth/login -> {loginResponse.StatusCode}");
            Console.WriteLine("Treść odpowiedzi: " + await loginResponse.Content.ReadAsStringAsync());
        }

        Console.WriteLine();
    }
}
