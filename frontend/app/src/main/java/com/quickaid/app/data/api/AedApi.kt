package com.quickaid.app.data.api

import com.quickaid.app.data.models.AedDto
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

// Interfejs API do operacji na AED
interface AedApi {

    // Pobiera wszystkie AED
    @GET("aed")
    suspend fun getAllAeds(): List<AedDto>

    // Pobiera AED po Id
    @GET("aed/{id}")
    suspend fun getAedById(@Path("id") id: Int): AedDto

    // Dodaje nowe AED
    @POST("aed")
    suspend fun addAed(@Body aed: AedDto): AedDto

    // Aktualizuje AED po Id
    @PUT("aed/{id}")
    suspend fun updateAed(
        @Path("id") id: Int,
        @Body aed: AedDto
    ): AedDto

    // Usuwa AED po Id
    @DELETE("aed/{id}")
    suspend fun deleteAed(@Path("id") id: Int): Response<Unit>
}
