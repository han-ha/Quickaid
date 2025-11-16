using System.Net.Http.Json;
using Quickaid.Models.DTO;

namespace QuickaidApiTests;

public static class QuizzesTests
{
    public static async Task Run(HttpClient client)
    {
        Console.WriteLine("=== Quizzes Tests ===");

        var quizzes = await client.GetFromJsonAsync<List<QuizDto>>("api/quizzes");
        Console.WriteLine($"GET /api/quizzes -> {quizzes?.Count} quizów");

        var newQuiz = new QuizDto { Title = "Test Quiz", Description = "Opis", NumberOfQuestions = 0 };
        var postResponse = await client.PostAsJsonAsync("api/quizzes", newQuiz);
        Console.WriteLine($"POST /api/quizzes -> {postResponse.StatusCode}");

        Console.WriteLine();
    }
}
