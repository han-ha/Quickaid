package com.quickaid.app.data.api

import com.quickaid.app.data.models.UserDto
import retrofit2.Response
import retrofit2.http.*

// Interfejs API do operacji administracyjnych na użytkownikach
interface AdminApi {

    // Pobiera listę wszystkich użytkowników
    @GET("users")
    suspend fun getUsers(): List<UserDto>

    // Usuwa użytkownika po Id
    @DELETE("users/{id}")
    suspend fun deleteUser(
        @Path("id") id: Int
    ): Response<Unit>

    // Aktualizuje dane użytkownika po Id
    @PUT("users/{id}")
    suspend fun updateUser(
        @Path("id") id: Int,
        @Body body: UserDto
    ): Response<Unit>
}
