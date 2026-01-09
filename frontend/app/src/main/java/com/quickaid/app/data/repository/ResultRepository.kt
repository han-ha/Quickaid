package com.quickaid.app.data.repository

import com.quickaid.app.data.api.ResultApi
import com.quickaid.app.data.models.ResultDto
import javax.inject.Inject

class ResultRepository @Inject constructor(private val api: ResultApi) {

    suspend fun getBestResult(quizId: Int): ResultDto? {
        return try {
            api.getBestResult(quizId)
        } catch (e: Exception) {
            null
        }
    }
}
