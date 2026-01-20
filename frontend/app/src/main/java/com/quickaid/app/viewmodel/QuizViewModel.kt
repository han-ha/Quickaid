package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.models.QuestionDto
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.data.repository.QuizRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class QuizViewModel @Inject constructor(
    private val repository: QuizRepository
) : ViewModel() {

    private val _quizzes = MutableStateFlow<List<QuizDto>>(emptyList())
    val quizzes: StateFlow<List<QuizDto>> = _quizzes

    private val _selectedQuiz = MutableStateFlow<QuizDto?>(null)
    val selectedQuiz: StateFlow<QuizDto?> = _selectedQuiz

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    private val _addSuccess = MutableStateFlow(false)
    val addSuccess: StateFlow<Boolean> = _addSuccess

    private val _updateSuccess = MutableStateFlow(false)
    val updateSuccess: StateFlow<Boolean> = _updateSuccess

    private val _deleteSuccess = MutableStateFlow(false)
    val deleteSuccess: StateFlow<Boolean> = _deleteSuccess

    fun resetAddState() { _addSuccess.value = false }
    fun resetUpdateState() { _updateSuccess.value = false }
    fun resetDeleteState() { _deleteSuccess.value = false }

    fun fetchQuizzes() {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try { _quizzes.value = repository.getQuizzes() }
            catch (e: Exception) { _error.value = e.message }
            finally { _isLoading.value = false }
        }
    }

    fun fetchQuizById(quizId: Int) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try { _selectedQuiz.value = repository.getQuizById(quizId) }
            catch (e: Exception) { _error.value = e.message }
            finally { _isLoading.value = false }
        }
    }

    fun updateQuiz(quizId: Int, quiz: QuizDto) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.updateQuiz(quizId, quiz)
                _updateSuccess.value = true
                _selectedQuiz.value = quiz
                fetchQuizzes()
            } catch (e: Exception) { _error.value = e.message }
            finally { _isLoading.value = false }
        }
    }

    fun deleteQuiz(quizId: Int) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.deleteQuiz(quizId)
                _deleteSuccess.value = true
                fetchQuizzes()
            } catch (e: Exception) { _error.value = e.message }
            finally { _isLoading.value = false }
        }
    }

    fun updateQuestions(newQuestions: List<QuestionDto>) {
        _selectedQuiz.value = _selectedQuiz.value?.copy(questions = newQuestions)
    }

    fun addQuiz(quiz: QuizDto, onSuccess: (Int) -> Unit) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                val createdQuiz = repository.addQuiz(quiz)
                _addSuccess.value = true
                fetchQuizzes()
                onSuccess(createdQuiz.id)
            } catch (e: Exception) { _error.value = e.message }
            finally { _isLoading.value = false }
        }
    }

}
