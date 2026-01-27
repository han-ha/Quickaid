package com.quickaid.app.data.repository

import com.quickaid.app.data.api.QuestionApi
import com.quickaid.app.data.models.QuestionDto
import retrofit2.Response
import javax.inject.Inject

// Repository do operacji na pytaniach
class QuestionRepository @Inject constructor(private val api: QuestionApi) {

    // Pobiera pytanie po Id
    suspend fun getQuestionById(id: Int): QuestionDto = api.getQuestionById(id)

    // Dodaje nowe pytanie
    suspend fun addQuestion(question: QuestionDto): QuestionDto = api.addQuestion(question)

    // Aktualizuje pytanie po Id
    suspend fun updateQuestion(id: Int, question: QuestionDto): QuestionDto =
        api.updateQuestion(id, question)

    // Usuwa pytanie powiązane z quizem
    suspend fun deleteQuestion(questionId: Int, quizId: Int): Response<Unit> =
        api.deleteQuestion(questionId, quizId)

    // Dodaje pytanie do konkretnego quizu
    suspend fun addQuestionToQuiz(quizId: Int, dto: QuestionDto): QuestionDto =
        api.addQuestionToQuiz(quizId, dto)
}
