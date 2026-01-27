package com.quickaid.app.data.api

import com.quickaid.app.data.models.QuizSubmissionDto
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.data.models.ResultDto
import retrofit2.Response
import retrofit2.http.*

// Interfejs API do operacji na quizach i ich wynikach
interface QuizApi {

    // Pobiera listę wszystkich quizów
    @GET("quizzes")
    suspend fun getQuizzes(): List<QuizDto>

    // Pobiera quiz po Id
    @GET("quizzes/{id}")
    suspend fun getQuizById(@Path("id") id: Int): QuizDto

    // Przesyła odpowiedzi użytkownika dla konkretnego quizu i zwraca wynik
    @POST("quizzes/{quizId}/submit")
    suspend fun submitQuiz(
        @Path("quizId") quizId: Int,
        @Body submission: QuizSubmissionDto
    ): ResultDto

    // Dodaje nowy quiz
    @POST("quizzes")
    suspend fun addQuiz(@Body quiz: QuizDto): QuizDto

    // Aktualizuje quiz po Id
    @PUT("quizzes/{id}")
    suspend fun updateQuiz(@Path("id") id: Int, @Body quiz: QuizDto): QuizDto

    // Usuwa quiz po Id
    @DELETE("quizzes/{id}")
    suspend fun deleteQuiz(@Path("id") id: Int): Response<Unit>
}
