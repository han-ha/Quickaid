package com.quickaid.app.data.models

data class AuthResponseDto(
    val success: Boolean,
    val message: String,
    val token: String?
)