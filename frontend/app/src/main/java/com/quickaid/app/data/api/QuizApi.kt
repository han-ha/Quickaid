package com.quickaid.app.data.api

import com.quickaid.app.data.models.QuizSubmissionDto
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.data.models.ResultDto
import retrofit2.Response
import retrofit2.http.*

interface QuizApi {

    @GET("quizzes")
    suspend fun getQuizzes(): List<QuizDto>

    @GET("quizzes/{id}")
    suspend fun getQuizById(@Path("id") id: Int): QuizDto

    @POST("quizzes/{quizId}/submit")
    suspend fun submitQuiz(
        @Path("quizId") quizId: Int,
        @Body submission: QuizSubmissionDto
    ): ResultDto

    @POST("quizzes")
    suspend fun addQuiz(@Body quiz: QuizDto): QuizDto

    @PUT("quizzes/{id}")
    suspend fun updateQuiz(@Path("id") id: Int, @Body quiz: QuizDto): QuizDto

    @DELETE("quizzes/{id}")
    suspend fun deleteQuiz(@Path("id") id: Int): Response<Unit>
}
