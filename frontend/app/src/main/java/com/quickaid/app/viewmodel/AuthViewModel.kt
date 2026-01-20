package com.quickaid.app.viewmodel

import AuthStateDto
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

    private val _authStateDto = MutableStateFlow<AuthStateDto>(AuthStateDto.Idle)
    val authStateDto: StateFlow<AuthStateDto> = _authStateDto

    fun login(username: String, password: String) {
        viewModelScope.launch {
            _authStateDto.value = AuthStateDto.Loading
            try {
                val response = repository.login(LoginDto(username, password))
                _authStateDto.value = AuthStateDto.Success(response)

                val token = response.token ?: ""
                val role = jwtUtils.getRole(token)
                val username = jwtUtils.getUsername(token)
                val userId = jwtUtils.getUserId(token)

                session.saveSession(token, role, username, userId)

            } catch (e: Exception) {
                _authStateDto.value = AuthStateDto.Error(e.message ?: "Nieznany błąd.")
            }
        }
    }

    fun register(username: String, email: String, password: String) {
        viewModelScope.launch {
            _authStateDto.value = AuthStateDto.Loading
            try {
                val response = repository.register(RegisterDto(username, email, password))
                _authStateDto.value = AuthStateDto.Success(response)
            } catch (e: Exception) {
                _authStateDto.value = AuthStateDto.Error(e.message ?: "Nieznany błąd.")
            }
        }
    }
}

