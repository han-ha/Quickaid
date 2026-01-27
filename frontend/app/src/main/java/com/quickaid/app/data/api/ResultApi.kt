package com.quickaid.app.data.api

import com.quickaid.app.data.models.ResultDto
import retrofit2.http.GET
import retrofit2.http.Path

// Interfejs API do operacji na wynikach quizów
interface ResultApi {

    // Pobiera najlepszy wynik użytkownika dla konkretnego quizu
    @GET("results/best/{quizId}")
    suspend fun getBestResult(
        @Path("quizId") quizId: Int
    ): ResultDto
}
