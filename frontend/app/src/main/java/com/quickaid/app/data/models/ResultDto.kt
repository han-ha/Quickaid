package com.quickaid.app.data.models

data class ResultDto(
    val id: Int,
    val userId: Int,
    val quizId: Int,
    val score: Int,
    val completedAt: String
)