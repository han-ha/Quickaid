package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.data.repository.QuizRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class QuizDetailsViewModel @Inject constructor(
    private val repository: QuizRepository
) : ViewModel() {

    // Przechowuje szczegóły quizu
    private val _quiz = MutableStateFlow<QuizDto?>(null)
    val quiz: StateFlow<QuizDto?> = _quiz

    // Stan ładowania
    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    // Przechowuje komunikat błędu
    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Pobiera quiz po jego ID i zapisuje go w _quiz
    fun fetchQuiz(id: Int) {
        viewModelScope.launch {
            _isLoading.value = true
            try {
                _quiz.value = repository.getQuizById(id)
                _error.value = null
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Wysyła odpowiedzi użytkownika do serwera dla konkretnego quizu
    fun submitQuiz(
        quizId: Int,
        answers: Map<Int, Int>
    ) {
        viewModelScope.launch {
            try {
                repository.submitQuiz(quizId, answers)
            } catch (e: Exception) {
                _error.value = e.message
            }
        }
    }

    // Oblicza punkty zdobyte w quizie oraz maksymalną liczbę punktów
    fun calculateQuizPoints(
        quiz: QuizDto,
        selectedAnswers: Map<Int, Int>
    ): Pair<Int, Int> {
        var points = 0
        val maxPoints = quiz.questions.size // Obecnie: maksymalna liczba punktów = liczba pytań

        quiz.questions.forEach { question ->
            val correctAnswerId = question.answers.firstOrNull { it.isCorrect }?.id
            val userAnswerId = selectedAnswers[question.id]

            // Dodanie punktu jeśli odpowiedź użytkownika jest poprawna
            if (userAnswerId != null && userAnswerId == correctAnswerId) {
                points++
            }
        }

        // Zwraca parę: (zdobyte punkty, maksymalna liczba punktów)
        return points to maxPoints
    }
}
