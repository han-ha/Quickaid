using System.Net.Http.Json;
using Quickaid.Models.DTO;

namespace QuickaidApiTests;

public static class ResultsTests
{
    public static async Task Run(HttpClient client)
    {
        Console.WriteLine("=== Results Tests ===");

        // pobierz pierwszego użytkownika
        var users = await client.GetFromJsonAsync<List<UserDto>>("api/users");
        var userId = users?.FirstOrDefault()?.Id ?? 1;

        // pobierz pierwszy quiz
        var quizzes = await client.GetFromJsonAsync<List<QuizDto>>("api/quizzes");
        var quizId = quizzes?.FirstOrDefault()?.Id ?? 1;

        // GET all results
        var getAllResponse = await client.GetAsync("api/results");
        if (getAllResponse.IsSuccessStatusCode)
        {
            var allResults = await getAllResponse.Content.ReadFromJsonAsync<List<ResultDto>>();
            Console.WriteLine($"GET /api/results -> {allResults?.Count} wyników");
        }
        else
        {
            Console.WriteLine($"GET /api/results -> {getAllResponse.StatusCode}");
            Console.WriteLine("Treść odpowiedzi: " + await getAllResponse.Content.ReadAsStringAsync());
        }

        // GET results by user
        var getUserResultsResponse = await client.GetAsync($"api/results/user/{userId}");
        if (getUserResultsResponse.IsSuccessStatusCode)
        {
            var userResults = await getUserResultsResponse.Content.ReadFromJsonAsync<List<ResultDto>>();
            Console.WriteLine($"GET /api/results/user/{userId} -> {userResults?.Count} wyników");
        }
        else
        {
            Console.WriteLine($"GET /api/results/user/{userId} -> {getUserResultsResponse.StatusCode}");
            Console.WriteLine("Treść odpowiedzi: " + await getUserResultsResponse.Content.ReadAsStringAsync());
        }

        // POST new result
        var newResult = new ResultDto { UserId = userId, QuizId = quizId, Score = 5 };
        var postResponse = await client.PostAsJsonAsync("api/results", newResult);
        Console.WriteLine($"POST /api/results -> {postResponse.StatusCode}");
        if (!postResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Treść odpowiedzi: " + await postResponse.Content.ReadAsStringAsync());
            Console.WriteLine();
            return;
        }

        var createdResult = await postResponse.Content.ReadFromJsonAsync<ResultDto>();
        if (createdResult == null)
        {
            Console.WriteLine("Nie udało się odczytać utworzonego wyniku.");
            Console.WriteLine();
            return;
        }

        // GET by id
        var getByIdResponse = await client.GetAsync($"api/results/{createdResult.Id}");
        if (getByIdResponse.IsSuccessStatusCode)
        {
            var resultById = await getByIdResponse.Content.ReadFromJsonAsync<ResultDto>();
            Console.WriteLine($"GET /api/results/{createdResult.Id} -> znaleziono wynik (Score={resultById?.Score})");
        }
        else
        {
            Console.WriteLine($"GET /api/results/{createdResult.Id} -> {getByIdResponse.StatusCode}");
            Console.WriteLine("Treść odpowiedzi: " + await getByIdResponse.Content.ReadAsStringAsync());
        }

        // PUT / update
        createdResult.Score += 1;
        var putResponse = await client.PutAsJsonAsync($"api/results/{createdResult.Id}", createdResult);
        Console.WriteLine($"PUT /api/results/{createdResult.Id} -> {putResponse.StatusCode}");
        if (!putResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Treść odpowiedzi: " + await putResponse.Content.ReadAsStringAsync());
        }

        // GET after update
        var getAfterUpdateResponse = await client.GetAsync($"api/results/{createdResult.Id}");
        if (getAfterUpdateResponse.IsSuccessStatusCode)
        {
            var updated = await getAfterUpdateResponse.Content.ReadFromJsonAsync<ResultDto>();
            Console.WriteLine($"GET po update -> wynik = {updated?.Score}");
        }
        else
        {
            Console.WriteLine($"GET po update -> {getAfterUpdateResponse.StatusCode}");
            Console.WriteLine("Treść odpowiedzi: " + await getAfterUpdateResponse.Content.ReadAsStringAsync());
        }

        // DELETE
        var deleteResponse = await client.DeleteAsync($"api/results/{createdResult.Id}");
        Console.WriteLine($"DELETE /api/results/{createdResult.Id} -> {deleteResponse.StatusCode}");
        if (!deleteResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Treść odpowiedzi: " + await deleteResponse.Content.ReadAsStringAsync());
        }

        // GET after delete
        var getAfterDeleteResponse = await client.GetAsync($"api/results/{createdResult.Id}");
        if (getAfterDeleteResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine("GET po delete -> rekord usunięty (404 oczekiwane)");
        }
        else
        {
            Console.WriteLine($"GET po delete -> {getAfterDeleteResponse.StatusCode}");
            Console.WriteLine("Treść odpowiedzi: " + await getAfterDeleteResponse.Content.ReadAsStringAsync());
        }

        Console.WriteLine();
    }
}
