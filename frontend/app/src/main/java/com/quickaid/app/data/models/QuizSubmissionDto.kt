package com.quickaid.app.data.models

// DTO odpowiedzi użytkownika do quizu
data class QuizSubmissionDto(
    val answers: Map<Int, Int>
)
