using Microsoft.EntityFrameworkCore;
using Quickaid.Models.Entities;

namespace Quickaid.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<AedPoint> AedPoints { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<UserQuizResult> UserQuizResults { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizQuestion>()
                .HasKey(q => new { q.QuizId, q.QuestionId });

            modelBuilder.Entity<Password>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Answer>()
                .HasOne<Question>()
                .WithMany()
                .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<UserQuizResult>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<UserQuizResult>()
                .HasOne<Quiz>()
                .WithMany()
                .HasForeignKey(r => r.QuizId);

            modelBuilder.Entity<Article>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.CreatedBy);

            modelBuilder.Entity<AedPoint>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.AddedBy);

            modelBuilder.Entity<QuizQuestion>()
                .HasKey(q => new { q.QuizId, q.QuestionId });

        }
    }
}
