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

    private val _quiz = MutableStateFlow<QuizDto?>(null)
    val quiz: StateFlow<QuizDto?> = _quiz

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

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

    fun calculateQuizPoints(
        quiz: QuizDto,
        selectedAnswers: Map<Int, Int>
    ): Pair<Int, Int> {
        var points = 0
        val maxPoints = quiz.questions.size

        quiz.questions.forEach { question ->
            val correctAnswerId = question.answers.firstOrNull { it.isCorrect }?.id
            val userAnswerId = selectedAnswers[question.id]

            if (userAnswerId != null && userAnswerId == correctAnswerId) {
                points++
            }
        }

        return points to maxPoints
    }
}
