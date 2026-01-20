package com.quickaid.app.data.api

import com.quickaid.app.data.models.AedDto
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

interface AedApi {

    @GET("aed")
    suspend fun getAllAeds(): List<AedDto>

    @GET("aed/{id}")
    suspend fun getAedById(@Path("id") id: Int): AedDto

    @POST("aed")
    suspend fun addAed(@Body aed: AedDto): AedDto

    @PUT("aed/{id}")
    suspend fun updateAed(
        @Path("id") id: Int,
        @Body aed: AedDto
    ): AedDto

    @DELETE("aed/{id}")
    suspend fun deleteAed(@Path("id") id: Int)
}
