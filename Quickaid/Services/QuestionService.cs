using Microsoft.EntityFrameworkCore;
using Quickaid.Data;
using Quickaid.Models.DTO;
using Quickaid.Services.Interfaces;
using Quickaid.Mapping.Interfaces;
using Quickaid.Models.Entities;

namespace Quickaid.Services
{
    // Serwis obsługujący pytania i powiązania pytań z quizami
    public class QuestionService(AppDbContext context, IQuestionMapper mapper, IAnswerMapper answerMapper) : IQuestionService
    {
        private readonly AppDbContext _context = context;
        private readonly IQuestionMapper _mapper = mapper;
        private readonly IAnswerMapper _answerMapper = answerMapper;

        // Zwraca wszystkie pytania z odpowiedziami
        public async Task<IEnumerable<QuestionDto>> GetAllAsync()
        {
            var questions = await _context.Questions.ToListAsync();
            var result = new List<QuestionDto>();

            foreach (var q in questions)
            {
                var answers = await _context.Answers
                    .Where(a => a.QuestionId == q.Id)
                    .ToListAsync();

                result.Add(new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Answers = [.. answers.Select(a => _answerMapper.ToDto(a))]
                });
            }

            return result;
        }

        // Zwraca pytanie po Id wraz z odpowiedziami
        public async Task<QuestionDto?> GetByIdAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return null;

            var answers = await _context.Answers
                .Where(a => a.QuestionId == id)
                .ToListAsync();

            return new QuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                Answers = [.. answers.Select(a => _answerMapper.ToDto(a))]
            };
        }

        // Dodaje nowe pytanie
        public async Task<QuestionDto> AddAsync(QuestionDto dto)
        {
            var entity = _mapper.ToEntity(dto);
            _context.Questions.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.ToDto(entity);
        }

        // Aktualizuje pytanie i jego odpowiedzi
        public async Task<QuestionDto?> UpdateAsync(int id, QuestionDto dto)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return null;

            // Aktualizacja tekstu pytania i liczby odpowiedzi
            question.QuestionText = dto.QuestionText;
            question.NumberOfAnswers = dto.Answers?.Count ?? 0;

            // Usuń stare odpowiedzi
            var currentAnswers = await _context.Answers
                .Where(a => a.QuestionId == id)
                .ToListAsync();
            if (currentAnswers.Count != 0)
                _context.Answers.RemoveRange(currentAnswers);

            // Dodaj nowe odpowiedzi
            var newAnswers = (dto.Answers ?? []).Select(a => new Answer
            {
                QuestionId = id,
                AnswerText = a.AnswerText,
                IsCorrect = a.IsCorrect
            }).ToList();
            if (newAnswers.Any())
                _context.Answers.AddRange(newAnswers);

            await _context.SaveChangesAsync();
            return _mapper.ToDto(question);
        }

        // Usuwa pytanie i jego powiązania z quizami
        public async Task<bool> DeleteAsync(int questionId, int quizId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null) return false;

            // Pobierz powiązanie pytania z tym konkretnym quizem
            var quizLink = await _context.QuizQuestions
                .FirstOrDefaultAsync(qq => qq.QuestionId == questionId && qq.QuizId == quizId);

            if (quizLink != null) // Jeśli istnieje powiązanie
            {
                // To je usuń
                _context.QuizQuestions.Remove(quizLink);

                // I zmniejsz licznik pytań w tym quizie i max score
                var quiz = await _context.Quizzes.FindAsync(quizId);
                if (quiz != null)
                {
                    quiz.NumberOfQuestions = Math.Max(0, (quiz.NumberOfQuestions ?? 0) - 1);
                    // Każde pytanie jest warte 1 pkt, obecnie ta kolumna nie jest przekazywana w DTO,
                    // ale dla łatwiejszego rozszerzenia modułu quizów jest aktualizowana
                    quiz.MaxScore = Math.Max(0, (quiz.MaxScore ?? 0) - 1);
                }
            }

            // Sprawdź, czy pytanie jest używane w jakimkolwiek innym quizie
            var stillUsed = await _context.QuizQuestions
                .AnyAsync(qq => qq.QuestionId == questionId);

            if (!stillUsed)
            {
                // Jeśli pytanie nie jest nigdzie więcej używane, to usuwamy je z bazy
                // Odpowiedzi zostaną usunięte kaskadowo
                _context.Questions.Remove(question);
            }

            // Zapis wszystkich zmian
            await _context.SaveChangesAsync();
            return true;
        }

        // Zwraca pytania dla konkretnego quizu
        public async Task<List<QuestionDto>> GetByQuizIdAsync(int quizId)
        {
            var quizQuestions = await _context.QuizQuestions
                .Where(qq => qq.QuizId == quizId)
                .ToListAsync();

            var result = new List<QuestionDto>();

            foreach (var qq in quizQuestions)
            {
                var question = await GetByIdAsync(qq.QuestionId);
                if (question != null)
                    result.Add(question);
            }

            return result;
        }

        // Dodaje pytanie do quizu wraz z odpowiedziami
        public async Task<QuestionDto> AddToQuizAsync(int quizId, QuestionDto dto)
        {
            var question = new Question
            {
                QuestionText = dto.QuestionText,
                CreatedAt = DateTime.UtcNow,
                NumberOfAnswers = dto.Answers?.Count ?? 0
            };

            // Dodanie pytania do bazy
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            // Tworzenie powiązania z quizem
            _context.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quizId,
                QuestionId = question.Id
            });

            // Dodawanie odpowiedzi
            if (dto.Answers != null && dto.Answers.Count != 0)
            {
                var answers = dto.Answers.Select(a => new Answer
                {
                    QuestionId = question.Id,
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                }).ToList();

                _context.Answers.AddRange(answers);
            }

            // Aktualizacja liczby pytań i max score w quizie
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz != null)
            {
                quiz.NumberOfQuestions = (quiz.NumberOfQuestions ?? 0) + 1;
                // Każde pytanie jest warte 1 pkt, obecnie ta kolumna nie jest przekazywana w DTO,
                // ale dla łatwiejszego rozszerzenia modułu quizów jest aktualizowana
                quiz.MaxScore = (quiz.MaxScore ?? 0) + 1;
            }

            await _context.SaveChangesAsync();

            // Pobranie pytania wraz z nowymi odpowiedziami
            var questionWithAnswers = await GetByIdAsync(question.Id);
            return questionWithAnswers!;
        }
    }
}
