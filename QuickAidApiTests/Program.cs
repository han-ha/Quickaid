using QuickaidApiTests;

Console.WriteLine("=== TESTY API ===\n");

var client = new HttpClient { BaseAddress = new Uri("https://localhost:44355/") };

await UsersTests.Run(client);
await AedTests.Run(client);
await ArticlesTests.Run(client);
await QuizzesTests.Run(client);
await QuestionsTests.Run(client);
await ResultsTests.Run(client);
await AuthTests.Run(client);

Console.WriteLine("\n=== KONIEC TESTÓW ===");
Console.ReadLine();
