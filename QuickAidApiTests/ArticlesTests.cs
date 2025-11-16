using System.Net.Http.Json;
using Quickaid.Models.DTO;

namespace QuickaidApiTests;

public static class ArticlesTests
{
    public static async Task Run(HttpClient client)
    {
        Console.WriteLine("=== Articles Tests ===");

        // Pobierz użytkowników, żeby ustalić CreatedBy
        var users = await client.GetFromJsonAsync<List<UserDto>>("api/users");
        var userId = users?.FirstOrDefault()?.Id ?? -1;

        if (userId == -1)
        {
            Console.WriteLine("Brak dostępnych użytkowników — pomijam test artykułów.");
            Console.WriteLine();
            return;
        }

        // GET all articles
        var articles = await client.GetFromJsonAsync<List<ArticleDto>>("api/articles");
        Console.WriteLine($"GET /api/articles -> {articles?.Count} artykułów");

        // POST example (używamy istniejącego użytkownika jako autora)
        var newArticle = new ArticleDto
        {
            Title = "Testowy artykuł",
            Content = "Treść testowa — automatyczny test API.",
            CreatedBy = userId
        };

        var postResponse = await client.PostAsJsonAsync("api/articles", newArticle);
        Console.WriteLine($"POST /api/articles -> {postResponse.StatusCode}");

        if (!postResponse.IsSuccessStatusCode)
        {
            var errorContent = await postResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Treść odpowiedzi: {errorContent}");
        }

        Console.WriteLine();
    }
}
