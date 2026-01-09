package com.quickaid.app.data.api

import com.quickaid.app.data.models.UserDto
import retrofit2.Response
import retrofit2.http.*

interface AdminApi {

    @GET("users")
    suspend fun getUsers(): List<UserDto>

    @DELETE("users/{id}")
    suspend fun deleteUser(
        @Path("id") id: Int
    ): Response<Unit>

    @PUT("users/{id}")
    suspend fun updateUser(
        @Path("id") id: Int,
        @Body body: UserDto
    ): Response<Unit>
}
