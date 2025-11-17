using System.Net;
using System.Net.Http.Json;
using Quickaid.Models.DTO;

namespace QuickaidApiTests;

public static class QuestionsTests
{
    public static async Task Run(HttpClient client)
    {
        Console.WriteLine("=== Questions Tests ===");

        // GET all questions
        var questions = await client.GetFromJsonAsync<List<QuestionDto>>("api/questions");
        Console.WriteLine($"GET /api/questions -> {questions?.Count} pytań");

        // POST example
        var newQuestion = new QuestionDto
        {
            QuestionText = "Co należy zrobić w przypadku zatrzymania krążenia?",
            Answers =
            [
                new() { AnswerText = "Wezwać pomoc" },
                new() { AnswerText = "Rozpocząć RKO" },
                new() { AnswerText = "Użyć AED" }
            ]
        };

        var postResponse = await client.PostAsJsonAsync("api/questions", newQuestion);
        Console.WriteLine($"POST /api/questions -> {postResponse.StatusCode}");

        QuestionDto? createdQuestion = null;
        if (postResponse.IsSuccessStatusCode)
        {
            createdQuestion = await postResponse.Content.ReadFromJsonAsync<QuestionDto>();
        }

        if (createdQuestion == null)
        {
            Console.WriteLine("Nie udało się utworzyć pytania, pomijam dalsze testy.\n");
            return;
        }

        // GET by id - sprawdzamy, że rekord istnieje przed DELETE
        var getBeforeDelete = await client.GetFromJsonAsync<QuestionDto>($"api/questions/{createdQuestion.Id}");
        if (getBeforeDelete == null)
        {
            Console.WriteLine("Błąd: rekord nie istnieje przed usunięciem – coś jest nie tak!");
            return;
        }
        Console.WriteLine($"GET /api/questions/{createdQuestion.Id} -> znaleziono");

        // PUT / update
        createdQuestion.QuestionText = "Zmodyfikowane pytanie testowe";
        var putResponse = await client.PutAsJsonAsync($"api/questions/{createdQuestion.Id}", createdQuestion);
        Console.WriteLine($"PUT /api/questions/{createdQuestion.Id} -> {putResponse.StatusCode}");

        // GET po update
        var getAfterUpdate = await client.GetFromJsonAsync<QuestionDto>($"api/questions/{createdQuestion.Id}");
        Console.WriteLine($"GET po update -> {getAfterUpdate?.QuestionText}");

        // DELETE
        var deleteResponse = await client.DeleteAsync($"api/questions/{createdQuestion.Id}");
        Console.WriteLine($"DELETE /api/questions/{createdQuestion.Id} -> {deleteResponse.StatusCode}");

        // GET po delete - sprawdzamy, że 404 wynika z braku rekordu
        var getAfterDeleteResponse = await client.GetAsync($"api/questions/{createdQuestion.Id}");
        if (getAfterDeleteResponse.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("GET po delete -> rekord usunięty (404 oczekiwane)");
        }
        else
        {
            Console.WriteLine($"GET po delete -> niespodziewany status: {getAfterDeleteResponse.StatusCode}");
        }

        Console.WriteLine();
    }
}
