package com.quickaid.app.data.models

// DTO dla wyniku logowania/rejestracji
data class AuthResponseDto(
    val success: Boolean,
    val message: String,
    val token: String?
)