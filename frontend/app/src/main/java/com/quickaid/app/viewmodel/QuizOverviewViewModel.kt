package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.repository.ResultRepository
import com.quickaid.app.data.models.ResultDto
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class QuizOverviewViewModel @Inject constructor(
    private val resultRepository: ResultRepository
) : ViewModel() {

    // Przechowuje najlepszy wynik dla danego quizu
    private val _bestResult = MutableStateFlow<ResultDto?>(null)
    val bestResult: StateFlow<ResultDto?> = _bestResult

    // Stan ładowania
    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    // Przechowuj komunikat błędu
    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Pobiera najlepszy wynik dla konkretnego quizu po jego ID
    fun fetchBestResult(quizId: Int) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                _bestResult.value = resultRepository.getBestResult(quizId)
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }
}
