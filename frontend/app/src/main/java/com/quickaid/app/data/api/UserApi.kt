package com.quickaid.app.data.api

import retrofit2.http.DELETE
import retrofit2.Response

// Interfejs API do operacji na aktualnie zalogowanym użytkowniku
interface UsersApi {

    // Usuwa konto aktualnie zalogowanego użytkownika
    @DELETE("users/me")
    suspend fun deleteMe(): Response<Unit>
}
