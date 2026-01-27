package com.quickaid.app.data.repository

import com.quickaid.app.data.api.AuthApi
import com.quickaid.app.data.models.LoginDto
import com.quickaid.app.data.models.RegisterDto
import com.quickaid.app.data.models.AuthResponseDto
import javax.inject.Inject

// Repository do operacji uwierzytelniania
class AuthRepository @Inject constructor(private val api: AuthApi) {

    // Loguje użytkownika przy użyciu danych z LoginDto
    suspend fun login(request: LoginDto): AuthResponseDto = api.login(request)

    // Rejestruje nowego użytkownika przy użyciu danych z RegisterDto
    suspend fun register(request: RegisterDto): AuthResponseDto = api.register(request)
}
