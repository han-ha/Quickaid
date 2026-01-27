package com.quickaid.app.data.api

import com.quickaid.app.data.models.LoginDto
import com.quickaid.app.data.models.RegisterDto
import com.quickaid.app.data.models.AuthResponseDto
import retrofit2.http.Body
import retrofit2.http.POST

// Interfejs API do operacji uwierzytelniania użytkowników
interface AuthApi {
    // Loguje użytkownika i zwraca token oraz informacje o sukcesie
    @POST("auth/login")
    suspend fun login(@Body request: LoginDto): AuthResponseDto

    // Rejestruje nowego użytkownika i zwraca token oraz informacje o sukcesie
    @POST("auth/register")
    suspend fun register(@Body request: RegisterDto): AuthResponseDto
}
