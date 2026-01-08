package com.quickaid.app.data.api

import com.quickaid.app.data.models.ResultDto
import retrofit2.http.GET
import retrofit2.http.Path

interface ResultApi {

    @GET("results/best/{quizId}")
    suspend fun getBestResult(
        @Path("quizId") quizId: Int
    ): ResultDto
}
