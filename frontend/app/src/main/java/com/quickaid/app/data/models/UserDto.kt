package com.quickaid.app.data.models

// DTO użytkownika
data class UserDto(
    val id: Int,
    val username: String,
    val role: String,
    val email: String
)