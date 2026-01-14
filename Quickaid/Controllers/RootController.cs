using Microsoft.AspNetCore.Mvc;
using Quickaid.Data;
using Quickaid.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Quickaid.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;
        private static readonly string[] value = ["Nie udało się połączyć z bazą."];

        [HttpGet]
        public IActionResult Get()
        {

            return Ok("Hello World, backend here!");
            //    var output = new List<string>();

            //    try
            //    {
            //        if (!_db.Database.CanConnect())
            //            return Ok(value);

            //        output.Add("Połączenie z bazą działa.");

            //        // USER
            //        var user = new User { Username = "TempUser", Email = "temp@example.com", Role = "user" };
            //        var existingUser = _db.Users.FirstOrDefault(u => u.Email == user.Email);
            //        if (existingUser != null) { _db.Users.Remove(existingUser); _db.SaveChanges(); }

            //        _db.Users.Add(user); _db.SaveChanges();
            //        output.Add($"User → Dodano ID={user.Id}");

            //        var userRead = _db.Users.First(u => u.Id == user.Id);
            //        output.Add($"User → Odczytano {userRead.Username}");

            //        userRead.Username = "ModifiedUser"; _db.SaveChanges();
            //        var userModified = _db.Users.First(u => u.Id == user.Id);
            //        output.Add($"User → Zmieniono na {userModified.Username}");

            //        _db.Users.Remove(userModified); _db.SaveChanges();
            //        output.Add($"User → Usunięto (istnieje? {_db.Users.Any(u => u.Id == userModified.Id)})");

            //        var persistentUser = new User { Username = "PersistentUser", Email = "persist@example.com", Role = "tester" };
            //        _db.Users.Add(persistentUser); _db.SaveChanges();
            //        output.Add($"User → Pozostawiono rekord testowy (ID={persistentUser.Id})");

            //        // PASSWORD
            //        var password = new Password { UserId = persistentUser.Id, HashedPassword = "HASH123", Salt = "SALT" };
            //        _db.Passwords.Add(password); _db.SaveChanges();
            //        var pwRead = _db.Passwords.First(p => p.Id == password.Id);
            //        pwRead.HashedPassword = "HASH999"; _db.SaveChanges();
            //        _db.Passwords.Remove(pwRead); _db.SaveChanges();
            //        output.Add($"Password → CRUD wykonano");

            //        var persistentPassword = new Password { UserId = persistentUser.Id, HashedPassword = "STATIC", Salt = "STATIC" };
            //        _db.Passwords.Add(persistentPassword); _db.SaveChanges();
            //        output.Add($"Password → Pozostawiono rekord testowy (ID={persistentPassword.Id})");

            //        // ARTICLE
            //        var article = new Article { Title = "Test Article", Content = "Initial content", CreatedBy = persistentUser.Id };
            //        _db.Articles.Add(article); _db.SaveChanges();
            //        var articleRead = _db.Articles.First(a => a.Id == article.Id);
            //        articleRead.Title = "Modified Article"; _db.SaveChanges();
            //        _db.Articles.Remove(_db.Articles.First(a => a.Id == article.Id)); _db.SaveChanges();
            //        var persistentArticle = new Article { Title = "Persistent Article", Content = "Zostaje", CreatedBy = persistentUser.Id };
            //        _db.Articles.Add(persistentArticle); _db.SaveChanges();
            //        output.Add($"Article → CRUD + rekord testowy ID={persistentArticle.Id}");

            //        // AED POINT
            //        var point = new AedPoint { Latitude = 50.123M, Longitude = 19.456M, Description = "Test point", AddedBy = persistentUser.Id };
            //        _db.AedPoints.Add(point); _db.SaveChanges();
            //        var pointRead = _db.AedPoints.First(p => p.Id == point.Id);
            //        pointRead.Description = "Modified point"; _db.SaveChanges();
            //        _db.AedPoints.Remove(pointRead); _db.SaveChanges();
            //        var persistentPoint = new AedPoint { Latitude = 51.000M, Longitude = 20.000M, Description = "Persistent AED", AddedBy = persistentUser.Id };
            //        _db.AedPoints.Add(persistentPoint); _db.SaveChanges();
            //        output.Add($"AedPoint → CRUD + rekord testowy ID={persistentPoint.Id}");

            //        // QUIZ
            //        var quiz = new Quiz { Title = "Test Quiz", Description = "Basic quiz" };
            //        _db.Quizzes.Add(quiz); _db.SaveChanges();
            //        var quizRead = _db.Quizzes.First(q => q.Id == quiz.Id);
            //        quizRead.Description = "Modified quiz"; _db.SaveChanges();
            //        _db.Quizzes.Remove(quizRead); _db.SaveChanges();
            //        var persistentQuiz = new Quiz { Title = "Persistent Quiz", Description = "Pozostaje" };
            //        _db.Quizzes.Add(persistentQuiz); _db.SaveChanges();
            //        output.Add($"Quiz → CRUD + rekord testowy ID={persistentQuiz.Id}");

            //        // QUESTION
            //        var question = new Question { QuestionText = "Test Question?" };
            //        _db.Questions.Add(question); _db.SaveChanges();
            //        var questionRead = _db.Questions.First(q => q.Id == question.Id);
            //        questionRead.QuestionText = "Modified Question?"; _db.SaveChanges();
            //        _db.Questions.Remove(questionRead); _db.SaveChanges();
            //        var persistentQuestion = new Question { QuestionText = "Persistent Question?" };
            //        _db.Questions.Add(persistentQuestion); _db.SaveChanges();
            //        output.Add($"Question → CRUD + rekord testowy ID={persistentQuestion.Id}");

            //        // ANSWER
            //        var answer = new Answer { QuestionId = persistentQuestion.Id, AnswerText = "Test Answer", IsCorrect = false };
            //        _db.Answers.Add(answer); _db.SaveChanges();
            //        var answerRead = _db.Answers.First(a => a.Id == answer.Id);
            //        answerRead.AnswerText = "Modified Answer"; _db.SaveChanges();
            //        _db.Answers.Remove(answerRead); _db.SaveChanges();
            //        var persistentAnswer = new Answer { QuestionId = persistentQuestion.Id, AnswerText = "Persistent Answer", IsCorrect = true };
            //        _db.Answers.Add(persistentAnswer); _db.SaveChanges();
            //        output.Add($"Answer → CRUD + rekord testowy ID={persistentAnswer.Id}");

            //        // QUIZ QUESTION
            //        var qq = new QuizQuestion { QuizId = persistentQuiz.Id, QuestionId = persistentQuestion.Id };
            //        _db.QuizQuestions.Add(qq); _db.SaveChanges();
            //        _db.QuizQuestions.Remove(qq); _db.SaveChanges();
            //        var persistentQQ = new QuizQuestion { QuizId = persistentQuiz.Id, QuestionId = persistentQuestion.Id };
            //        _db.QuizQuestions.Add(persistentQQ); _db.SaveChanges();
            //        output.Add($"QuizQuestion → CRUD + rekord testowy");

            //        // USER QUIZ RESULT
            //        var result = new Result { UserId = persistentUser.Id, QuizId = persistentQuiz.Id, Score = 85 };
            //        _db.UserQuizResults.Add(result); _db.SaveChanges();
            //        var resultRead = _db.UserQuizResults.First(r => r.Id == result.Id);
            //        resultRead.Score = 90; _db.SaveChanges();
            //        _db.UserQuizResults.Remove(resultRead); _db.SaveChanges();
            //        var persistentResult = new Result { UserId = persistentUser.Id, QuizId = persistentQuiz.Id, Score = 100 };
            //        _db.UserQuizResults.Add(persistentResult); _db.SaveChanges();
            //        output.Add($"Result → CRUD + rekord testowy ID={persistentResult.Id}");

            //    }
            //    catch (Exception ex)
            //    {
            //        output.Add($"Błąd testu CRUD: {ex.Message}");
            //        if (ex.InnerException != null)
            //            output.Add($"Szczegóły: {ex.InnerException.Message}");
            //    }

            //    return Ok(output);

            
        }
}
}
