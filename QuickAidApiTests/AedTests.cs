using System.Net.Http.Json;
using Quickaid.Models.DTO;

namespace QuickaidApiTests;

public static class AedTests
{
    public static async Task Run(HttpClient client)
    {
        Console.WriteLine("=== AED Tests ===");

        // GET all AED points
        var points = await client.GetFromJsonAsync<List<InternalAedDto>>("api/aed");
        Console.WriteLine($"GET /api/aed -> {points?.Count} punktów AED");

        // Przykładowy POST (dodanie punktu)
        var newPoint = new InternalAedDto { Latitude = 50.0m, Longitude = 20.0m, Description = "Test AED", Verified = false };
        var postResponse = await client.PostAsJsonAsync("api/aed", newPoint);
        Console.WriteLine($"POST /api/aed -> {postResponse.StatusCode}");

        Console.WriteLine();
    }
}
