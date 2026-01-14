package com.quickaid.app.data.api

import com.quickaid.app.data.models.QuestionDto
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

interface QuestionApi {
    @GET("questions/{id}")
    suspend fun getQuestionById(@Path("id") id: Int): QuestionDto

    @POST("questions")
    suspend fun addQuestion(@Body question: QuestionDto): QuestionDto

    @PUT("questions/{id}")
    suspend fun updateQuestion(
        @Path("id") id: Int,
        @Body question: QuestionDto
    ): QuestionDto

    @DELETE("questions/{questionId}/quiz/{quizId}")
    suspend fun deleteQuestion(
        @Path("questionId") questionId: Int,
        @Path("quizId") quizId: Int
    ): Response<Unit>


    @POST("quizzes/{quizId}/questions")
    suspend fun addQuestionToQuiz(
        @Path("quizId") quizId: Int,
        @Body dto: QuestionDto
    ): QuestionDto

}
