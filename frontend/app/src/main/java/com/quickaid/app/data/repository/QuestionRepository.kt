package com.quickaid.app.data.repository

import com.quickaid.app.data.api.QuestionApi
import com.quickaid.app.data.models.QuestionDto
import retrofit2.Response
import javax.inject.Inject

class QuestionRepository @Inject constructor(private val api: QuestionApi) {

    suspend fun getQuestionById(id: Int): QuestionDto = api.getQuestionById(id)

    suspend fun addQuestion(question: QuestionDto): QuestionDto = api.addQuestion(question)

    suspend fun updateQuestion(id: Int, question: QuestionDto): QuestionDto =
        api.updateQuestion(id, question)

    suspend fun deleteQuestion(questionId: Int, quizId: Int): Response<Unit> {
        return api.deleteQuestion(questionId, quizId)
    }

    suspend fun addQuestionToQuiz(quizId: Int, dto: QuestionDto): QuestionDto {
        return api.addQuestionToQuiz(quizId, dto)
    }

}
