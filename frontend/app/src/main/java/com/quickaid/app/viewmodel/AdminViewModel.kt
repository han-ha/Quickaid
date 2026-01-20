package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.api.AdminApi
import com.quickaid.app.data.datastore.SessionDataStore
import com.quickaid.app.data.models.UserDto
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class AdminViewModel @Inject constructor(
    private val adminApi: AdminApi,
    private val sessionDataStore: SessionDataStore
) : ViewModel() {

    private val _users = MutableStateFlow<List<UserDto>>(emptyList())
    val users: StateFlow<List<UserDto>> = _users.asStateFlow()

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading.asStateFlow()

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error.asStateFlow()

    private val _currentUserId = MutableStateFlow<Int?>(null)
    val currentUserId: StateFlow<Int?> = _currentUserId.asStateFlow()

    init {
        viewModelScope.launch {
            sessionDataStore.userId.map { it?.toIntOrNull() }.collect {
                _currentUserId.value = it
            }
        }
    }

    fun fetchUsers() {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                _users.value = adminApi.getUsers().sortedBy { it.username.lowercase() }
            } catch (e: Exception) {
                _error.value = e.message ?: "Nie udało się pobrać użytkowników"
            } finally {
                _isLoading.value = false
            }
        }
    }

    fun deleteUser(user: UserDto) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                val response = adminApi.deleteUser(user.id)
                if (response.isSuccessful) fetchUsers()
                else _error.value = "Nie udało się usunąć użytkownika (${response.code()})"
            } catch (e: Exception) {
                _error.value = e.message ?: "Błąd podczas usuwania użytkownika"
            } finally {
                _isLoading.value = false
            }
        }
    }

    fun updateUser(request: UserDto, onSuccess: () -> Unit) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                val response = adminApi.updateUser(request.id, request)
                if (response.isSuccessful) {
                    fetchUsers()
                    onSuccess()
                } else {
                    _error.value = "Nie udało się zaktualizować użytkownika (${response.code()})"
                }
            } catch (e: Exception) {
                _error.value = e.message ?: "Błąd podczas aktualizacji użytkownika"
            } finally {
                _isLoading.value = false
            }
        }
    }
}
