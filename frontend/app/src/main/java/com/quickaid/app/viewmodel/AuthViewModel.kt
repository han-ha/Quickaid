package com.quickaid.app.viewmodel

import AuthState
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.datastore.SessionDataStore
import com.quickaid.app.data.models.LoginDto
import com.quickaid.app.data.models.RegisterDto
import com.quickaid.app.data.repository.AuthRepository
import com.quickaid.app.util.JwtUtils
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class AuthViewModel @Inject constructor(
    private val repository: AuthRepository,
    private val session: SessionDataStore,
    private val jwtUtils: JwtUtils
) : ViewModel() {

    // Stan logowania/rejestracji
    private val _authState = MutableStateFlow<AuthState>(AuthState.Idle)
    val authState: StateFlow<AuthState> = _authState

    // Funkcja logowania
    fun login(username: String, password: String) {
        viewModelScope.launch {
            _authState.value = AuthState.Loading
            try {
                val response = repository.login(LoginDto(username, password))
                _authState.value = AuthState.Success(response)

                val token = response.token ?: ""
                val role = jwtUtils.getRole(token)
                val username = jwtUtils.getUsername(token)
                val userId = jwtUtils.getUserId(token)

                session.saveSession(token, role, username, userId)

            } catch (e: Exception) {
                _authState.value = AuthState.Error(e.message ?: "Nieznany błąd.")
            }
        }
    }

    // Funkcja rejestracji
    fun register(username: String, email: String, password: String, confirmPassword: String) {
        viewModelScope.launch {
            _authState.value = AuthState.Loading
            try {
                val response = repository.register(RegisterDto(username, email, password, confirmPassword))
                _authState.value = AuthState.Success(response)
            } catch (e: Exception) {
                _authState.value = AuthState.Error(e.message ?: "Nieznany błąd.")
            }
        }
    }
}

