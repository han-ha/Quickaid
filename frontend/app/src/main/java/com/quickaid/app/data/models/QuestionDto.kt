package com.quickaid.app.data.models

data class QuestionDto(
    val id: Int = 0,
    val quizId: Int,
    val questionText: String,
    val answers: List<AnswerDto> = emptyList()
)
