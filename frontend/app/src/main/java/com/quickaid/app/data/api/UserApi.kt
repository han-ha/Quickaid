package com.quickaid.app.data.api

import retrofit2.http.DELETE

interface UsersApi {
    @DELETE("users/me")
    suspend fun deleteMe(): retrofit2.Response<Unit>
}
