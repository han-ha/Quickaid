package com.quickaid.app.data.repository

import com.quickaid.app.data.api.QuizApi
import com.quickaid.app.data.models.QuizSubmissionDto
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.data.models.ResultDto
import javax.inject.Inject

// Repository do operacji na quizach
class QuizRepository @Inject constructor(private val api: QuizApi) {

    // Pobiera wszystkie quizy
    suspend fun getQuizzes(): List<QuizDto> = api.getQuizzes()

    // Pobiera quiz po Id
    suspend fun getQuizById(id: Int): QuizDto = api.getQuizById(id)

    // Przesyła odpowiedzi użytkownika dla konkretnego quizu i zwraca wynik
    suspend fun submitQuiz(
        quizId: Int,
        answers: Map<Int, Int>
    ): ResultDto = api.submitQuiz(quizId, QuizSubmissionDto(answers))

    // Dodaje nowy quiz
    suspend fun addQuiz(quiz: QuizDto): QuizDto = api.addQuiz(quiz)

    // Aktualizuje istniejący quiz po Id
    suspend fun updateQuiz(quizId: Int, quiz: QuizDto): QuizDto = api.updateQuiz(quizId, quiz)

    // Usuwa quiz po Id
    suspend fun deleteQuiz(quizId: Int) = api.deleteQuiz(quizId)
}
