package com.quickaid.app.data.models

// DTO dla odpowiedzi
data class AnswerDto(
    val id: Int,
    val answerText: String,
    val isCorrect: Boolean
)