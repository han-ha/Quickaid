package com.quickaid.app.data.repository

import com.quickaid.app.data.api.ResultApi
import com.quickaid.app.data.models.ResultDto
import javax.inject.Inject

// Repository do operacji na wynikach quizów
class ResultRepository @Inject constructor(private val api: ResultApi) {

    // Pobiera najlepszy wynik dla konkretnego quizu
    // Zwraca null jeśli wystąpił błąd lub brak wyniku
    suspend fun getBestResult(quizId: Int): ResultDto? {
        return try {
            api.getBestResult(quizId)
        } catch (e: Exception) {
            null
        }
    }
}
