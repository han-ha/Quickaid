package com.quickaid.app.data.models

// DTO dla pytania
data class QuestionDto(
    val id: Int = 0,
    val quizId: Int,
    val questionText: String,
    val answers: List<AnswerDto> = emptyList()
)
