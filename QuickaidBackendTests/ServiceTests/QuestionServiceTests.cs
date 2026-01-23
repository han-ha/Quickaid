using Microsoft.EntityFrameworkCore;
using Moq;
using Quickaid.Data;
using Quickaid.Mapping.Interfaces;
using Quickaid.Models.DTO;
using Quickaid.Services;
using QuickaidBackendTests.TestHelpers;

namespace QuickaidBackendTests.ServiceTests
{
    [TestClass]
    public class QuestionServiceTests
    {
        private AppDbContext _db = null!;
        private Mock<IQuestionMapper> _questionMapperMock = null!;
        private Mock<IAnswerMapper> _answerMapperMock = null!;
        private QuestionService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _db = DbHelper.CreateInMemoryDb();
            DbHelper.SeedQuizzes(_db);

            _questionMapperMock = new Mock<IQuestionMapper>();
            _answerMapperMock = new Mock<IAnswerMapper>();

            // Mapowanie
            _questionMapperMock.Setup(m => m.ToDto(It.IsAny<Quickaid.Models.Entities.Question>()))
                .Returns<Quickaid.Models.Entities.Question>(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Answers = [.. _db.Answers
                        .Where(a => a.QuestionId == q.Id)
                        .Select(a => new AnswerDto
                        {
                            Id = a.Id,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        })]
                });

            _service = new QuestionService(_db, _questionMapperMock.Object, _answerMapperMock.Object);
        }

        [TestMethod]
        // Test GetAllAsync - zwraca wszystkie pytania z bazy wraz z odpowiedziami
        public async Task GetAllAsync_ReturnsAllQuestions()
        {
            var result = await _service.GetAllAsync();

            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(q => q.QuestionText == "Pytanie 10"));
            Assert.IsTrue(result.Any(q => q.Answers.Count != 0));
        }

        [TestMethod]
        // Test GetByIdAsync - zwraca pytanie dla istniejącego Id lub null dla nieistniejącego
        public async Task GetByIdAsync_ReturnsQuestionOrNull()
        {
            var found = await _service.GetByIdAsync(10);
            Assert.IsNotNull(found);
            Assert.AreEqual(10, found!.Id);
            Assert.IsNotEmpty(found.Answers);

            var missing = await _service.GetByIdAsync(99);
            Assert.IsNull(missing);
        }

        [TestMethod]
        // Test AddAsync - dodaje nowe pytanie wraz z odpowiedziami i zwraca DTO
        public async Task AddAsync_AddsQuestion()
        {
            var dto = new QuestionDto
            {
                QuestionText = "Nowe pytanie",
                Answers =
                [
                    new AnswerDto { AnswerText = "A", IsCorrect = true },
                    new AnswerDto { AnswerText = "B", IsCorrect = false }
                ]
            };

            // Mapowanie
            _questionMapperMock.Setup(m => m.ToEntity(It.IsAny<QuestionDto>())).Returns<QuestionDto>(d =>
                new Quickaid.Models.Entities.Question
                {
                    QuestionText = d.QuestionText,
                    NumberOfAnswers = d.Answers?.Count ?? 0
                }
            );

            _questionMapperMock.Setup(m => m.ToDto(It.IsAny<Quickaid.Models.Entities.Question>())).Returns<Quickaid.Models.Entities.Question>(q =>
                new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Answers = dto.Answers
                }
            );

            var result = await _service.AddAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Nowe pytanie", result.QuestionText);
            Assert.HasCount(2, result.Answers);

            var inDb = await _db.Questions.FirstOrDefaultAsync(q => q.QuestionText == "Nowe pytanie");
            Assert.IsNotNull(inDb);
            Assert.AreEqual(2, inDb.NumberOfAnswers);
        }

    }
}
