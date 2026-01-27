package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.models.QuestionDto
import com.quickaid.app.data.repository.QuestionRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class QuestionViewModel @Inject constructor(
    private val repository: QuestionRepository
) : ViewModel() {

    // Stan ładowania
    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    // Przechowuje komunikat błędu
    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Flagi sukcesu operacji
    private val _saveSuccess = MutableStateFlow(false)
    val saveSuccess: StateFlow<Boolean> = _saveSuccess

    private val _deleteSuccess = MutableStateFlow(false)
    val deleteSuccess: StateFlow<Boolean> = _deleteSuccess
    fun resetDeleteState() { _deleteSuccess.value = false }

    // Funkcja ustawiająca flagę sukcesu zapisu na true
    private fun notifySuccess() {
        _saveSuccess.value = true
    }

    // Resetuje flagę sukcesu zapisu
    fun resetSaveState() { _saveSuccess.value = false }

    // Aktualnie wybrane pytanie
    private val _question = MutableStateFlow<QuestionDto?>(null)
    val question: StateFlow<QuestionDto?> = _question

    // Pobiera pytanie po jego ID i ustawia je w _question
    fun fetchQuestionById(questionId: Int) {
        _isLoading.value = true
        viewModelScope.launch {
            try {
                _question.value = repository.getQuestionById(questionId)
                _error.value = null
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Dodaje nowe pytanie do bazy danych
    fun addQuestion(question: QuestionDto) {
        _isLoading.value = true
        viewModelScope.launch {
            try {
                repository.addQuestion(question)
                _error.value = null
                notifySuccess()
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Aktualizuje istniejące pytanie po ID
    fun updateQuestion(questionId: Int, question: QuestionDto) {
        _isLoading.value = true
        viewModelScope.launch {
            try {
                repository.updateQuestion(questionId, question)
                fetchQuestionById(questionId)
                _error.value = null
                notifySuccess()
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Dodaje pytanie do konkretnego quizu
    fun addQuestionToQuiz(
        quizId: Int,
        dto: QuestionDto,
        onSuccess: () -> Unit
    ) {
        _isLoading.value = true
        viewModelScope.launch {
            try {
                repository.addQuestionToQuiz(quizId, dto)
                _error.value = null
                notifySuccess()
                onSuccess()
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Usuwa pytanie z quizu po ID
    fun deleteQuestion(questionId: Int, quizId: Int) {
        _isLoading.value = true
        viewModelScope.launch {
            try {
                val response = repository.deleteQuestion(questionId, quizId)
                if (response.isSuccessful) {
                    _deleteSuccess.value = true
                } else {
                    _error.value = "Nie udało się usunąć pytania"
                }
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }
}
