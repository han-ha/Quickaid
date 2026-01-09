package com.quickaid.app.data.repository

import com.quickaid.app.data.api.AuthApi
import com.quickaid.app.data.models.LoginDto
import com.quickaid.app.data.models.RegisterDto
import com.quickaid.app.data.models.AuthResponseDto
import javax.inject.Inject

class AuthRepository @Inject constructor(private val api: AuthApi) {
    suspend fun login(request: LoginDto): AuthResponseDto = api.login(request)
    suspend fun register(request: RegisterDto): AuthResponseDto = api.register(request)
}
