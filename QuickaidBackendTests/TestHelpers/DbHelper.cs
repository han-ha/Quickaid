using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Models.Entities;
using System;
using System.Collections.Generic;

namespace QuickaidBackendTests.TestHelpers
{
    public static class DbHelper
    {
        public static AppDbContext CreateInMemoryDb(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        public static void SeedResults(AppDbContext db)
        {
            db.UserQuizResults.AddRange(
                new Result { Id = 1, UserId = 1, QuizId = 1, Score = 80, CompletedAt = DateTime.UtcNow.AddDays(-1) },
                new Result { Id = 2, UserId = 1, QuizId = 1, Score = 90, CompletedAt = DateTime.UtcNow },
                new Result { Id = 3, UserId = 2, QuizId = 1, Score = 70, CompletedAt = DateTime.UtcNow }
            );
            db.SaveChanges();
        }

        public static void SeedQuizzes(AppDbContext db)
        {
            // quizy
            var quiz1 = new Quiz { Id = 1, Title = "Quiz 1", NumberOfQuestions = 2, MaxScore = 2 };
            var quiz2 = new Quiz { Id = 2, Title = "Quiz 2", NumberOfQuestions = 1, MaxScore = 1 };
            db.Quizzes.AddRange(quiz1, quiz2);

            // pytania
            var q10 = new Question { Id = 10, QuestionText = "Pytanie 10", NumberOfAnswers = 2 };
            var q20 = new Question { Id = 20, QuestionText = "Pytanie 20", NumberOfAnswers = 2 };
            var q30 = new Question { Id = 30, QuestionText = "Pytanie 30", NumberOfAnswers = 2 };
            db.Questions.AddRange(q10, q20, q30);

            // powiązania quiz <-> pytanie
            db.QuizQuestions.AddRange(
                new QuizQuestion { QuizId = 1, QuestionId = 10 },
                new QuizQuestion { QuizId = 1, QuestionId = 20 },
                new QuizQuestion { QuizId = 2, QuestionId = 30 }
            );

            // odpowiedzi
            db.Answers.AddRange(
                new Answer { Id = 1, QuestionId = 10, AnswerText = "A", IsCorrect = true },
                new Answer { Id = 2, QuestionId = 10, AnswerText = "B", IsCorrect = false },

                new Answer { Id = 3, QuestionId = 20, AnswerText = "A", IsCorrect = true },
                new Answer { Id = 4, QuestionId = 20, AnswerText = "B", IsCorrect = false },

                new Answer { Id = 5, QuestionId = 30, AnswerText = "A", IsCorrect = true },
                new Answer { Id = 6, QuestionId = 30, AnswerText = "B", IsCorrect = false }
            );

            db.SaveChanges();
        }

        public static void SeedUsers(AppDbContext db)
        {
            db.Users.AddRange(
                new User { Id = 1, Username = "admin", Email = "a@test.com", Role = "admin" },
                new User { Id = 2, Username = "user", Email = "u@test.com", Role = "user" }
            );
            db.SaveChanges();
        }

        public static void SeedArticles(AppDbContext db)
        {
            db.Articles.AddRange(
                new Article { Id = 1, Title = "Article 1", Content = "Content 1", CreatedBy = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Article { Id = 2, Title = "Article 2", Content = "Content 2", CreatedBy = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            db.SaveChanges();
        }

        public static void SeedAeds(AppDbContext db)
        {
            db.AedPoints.AddRange(
                // Internal (dodane przez użytkownika, bez ExternalId)
                new AedPoint
                {
                    Id = 1,
                    Latitude = 50.061m,
                    Longitude = 19.938m,
                    Description = "AED 1 w centrum",
                    Verified = true,
                    UpdatedAt = DateTime.UtcNow
                },
                new AedPoint
                {
                    Id = 2,
                    Latitude = 50.064m,
                    Longitude = 19.945m,
                    Description = "AED 2 przy szkole",
                    Verified = false,
                    UpdatedAt = DateTime.UtcNow
                },

                // Combined (modyfikacja AED z ExternalId)
                new AedPoint
                {
                    Id = 3,
                    Latitude = 50.067m,
                    Longitude = 19.950m,
                    Description = "AED 3 w parku (Combined)",
                    Verified = true,
                    ExternalId = 12345,
                    UpdatedAt = DateTime.UtcNow
                },

                // Internal (bez ExternalId)
                new AedPoint
                {
                    Id = 4,
                    Latitude = 50.070m,
                    Longitude = 19.955m,
                    Description = "AED 4 w szpitalu",
                    Verified = true,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            db.SaveChanges();
        }
    }
}
