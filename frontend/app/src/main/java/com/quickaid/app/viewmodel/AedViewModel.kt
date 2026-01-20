package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.models.AedDto
import com.quickaid.app.data.repository.AedRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class AedViewModel @Inject constructor(
    private val repository: AedRepository
) : ViewModel() {

    private val _aeds = MutableStateFlow<List<AedDto>>(emptyList())
    val aeds: StateFlow<List<AedDto>> = _aeds

    private val _selectedAed = MutableStateFlow<AedDto?>(null)
    val selectedAed: StateFlow<AedDto?> = _selectedAed

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    private val _addSuccess = MutableStateFlow(false)
    val addSuccess: StateFlow<Boolean> = _addSuccess

    private val _updateSuccess = MutableStateFlow(false)
    val updateSuccess: StateFlow<Boolean> = _updateSuccess

    fun resetAddSuccess() { _addSuccess.value = false }
    fun resetUpdateSuccess() { _updateSuccess.value = false }
    fun clearSelected() { _selectedAed.value = null }

    fun fetchAeds() {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                _aeds.value = repository.getAllAeds()
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się pobrać AED"
            } finally { _isLoading.value = false }
        }
    }

    fun fetchAedById(id: Int) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                _selectedAed.value = repository.getAedById(id)
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się pobrać AED"
            } finally { _isLoading.value = false }
        }
    }

    fun addAed(aed: AedDto) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.addAed(aed)
                _addSuccess.value = true
                fetchAeds()
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się dodać AED"
            } finally { _isLoading.value = false }
        }
    }

    fun updateAed(aed: AedDto) {
        val idForBackend = aed.id ?: 0
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.updateAed(idForBackend, aed)
                _updateSuccess.value = true
                fetchAeds()
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się zapisać AED"
            } finally { _isLoading.value = false }
        }
    }

    fun deleteAed(id: Int, onSuccess: () -> Unit = {}) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.deleteAed(id)
                fetchAeds()
                onSuccess()
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się usunąć AED"
            } finally { _isLoading.value = false }
        }
    }
}
