package com.quickaid.app.data.api

import com.quickaid.app.data.models.AedDto
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface AedApi {

    @GET("aed")
    suspend fun getAllAeds(): List<AedDto>

    @POST("api/aed/save")
    suspend fun saveAed(
        @Body aed: AedDto
    ): AedDto

}
