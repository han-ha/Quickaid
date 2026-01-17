package com.quickaid.app.data.api

import com.quickaid.app.data.models.AedDto
import retrofit2.http.GET

interface AedApi {

    @GET("aed")
    suspend fun getAllAeds(): List<AedDto>

    @GET("aed/internal")
    suspend fun getInternalAeds(): List<AedDto>

    @GET("aed/external")
    suspend fun getExternalAeds(): List<AedDto>

}
