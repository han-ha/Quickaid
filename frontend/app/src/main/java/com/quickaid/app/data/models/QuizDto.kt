package com.quickaid.app.data.models

// DTO dla quizu
data class QuizDto(
    val id: Int,
    val title: String,
    val description: String,
    val numberOfQuestions: Int = 0,
    val questions: List<QuestionDto> = emptyList()
)