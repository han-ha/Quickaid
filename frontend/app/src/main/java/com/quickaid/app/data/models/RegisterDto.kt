package com.quickaid.app.data.models

// DTO rejestracji
data class RegisterDto(
    val username: String,
    val email: String,
    val password: String,
    val confirmPassword: String
)
