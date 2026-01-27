package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.models.AedDto
import com.quickaid.app.data.repository.AedRepository
import com.quickaid.app.enums.AedType
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class AedViewModel @Inject constructor(
    private val repository: AedRepository
) : ViewModel() {

    // Lista wszystkich AED
    private val _aeds = MutableStateFlow<List<AedDto>>(emptyList())
    val aeds: StateFlow<List<AedDto>> = _aeds

    // Aktualnie wybrane AED
    private val _selectedAed = MutableStateFlow<AedDto?>(null)
    val selectedAed: StateFlow<AedDto?> = _selectedAed

    // Stan ładowania
    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    // Przechowuje komunikat błędu
    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Flagi sukcesu operacji
    private val _addSuccess = MutableStateFlow(false)
    val addSuccess: StateFlow<Boolean> = _addSuccess

    private val _updateSuccess = MutableStateFlow(false)
    val updateSuccess: StateFlow<Boolean> = _updateSuccess

    private val _deleteSuccess = MutableStateFlow(false)
    val deleteSuccess: StateFlow<Boolean> = _deleteSuccess

    // Funkcje resetujące flagi sukcesu i zaznaczenie AED
    fun resetAddSuccess() { _addSuccess.value = false }
    fun resetUpdateSuccess() { _updateSuccess.value = false }
    fun resetDeleteSuccess() { _deleteSuccess.value = false }
    fun clearSelected() { _selectedAed.value = null }

    // Pobiera wszystkie AED
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

    // Zaznacza wybrane AED
    fun selectAed(aed: AedDto) {
        _selectedAed.value = aed
    }

    // Dodaje nowe AED
    fun addAed(aed: AedDto) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                if (aed.type != AedType.Internal) {
                    _error.value = "Można dodawać tylko AED typu Internal"
                    return@launch
                }
                val saved = repository.addAed(aed)
                _selectedAed.value = saved
                _addSuccess.value = true
                fetchAeds()
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się dodać AED"
            } finally { _isLoading.value = false }
        }
    }

    // Aktualizuje istniejące AED
    fun updateAed(aed: AedDto) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                if (aed.type != AedType.Internal) {
                    _error.value = "Nie można edytować AED zewnętrznych"
                    return@launch
                }
                val result = repository.updateAed(
                    aed.id ?: throw IllegalArgumentException("AED musi mieć ID"),
                    aed
                )
                _selectedAed.value = result
                _updateSuccess.value = true
                fetchAeds()
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się zapisać AED"
            } finally { _isLoading.value = false }
        }
    }

    // Usuwa AED po ID
    fun deleteAed(id: Int) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.deleteAed(id)
                _deleteSuccess.value = true
                fetchAeds()
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się usunąć AED"
            } finally { _isLoading.value = false }
        }
    }
}
