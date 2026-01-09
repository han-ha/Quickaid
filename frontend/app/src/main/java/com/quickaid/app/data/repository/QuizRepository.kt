package com.quickaid.app.data.repository

import com.quickaid.app.data.api.QuizApi
import com.quickaid.app.data.models.QuizSubmissionDto
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.data.models.ResultDto
import javax.inject.Inject

class QuizRepository @Inject constructor(private val api: QuizApi) {

    suspend fun getQuizzes(): List<QuizDto> = api.getQuizzes()

    suspend fun getQuizById(id: Int): QuizDto = api.getQuizById(id)

    suspend fun submitQuiz(
        quizId: Int,
        answers: Map<Int, Int>
    ): ResultDto = api.submitQuiz(quizId, QuizSubmissionDto(answers))

    suspend fun addQuiz(quiz: QuizDto): QuizDto = api.addQuiz(quiz)

    suspend fun updateQuiz(quizId: Int, quiz: QuizDto): QuizDto = api.updateQuiz(quizId, quiz)

    suspend fun deleteQuiz(quizId: Int) = api.deleteQuiz(quizId)
}
