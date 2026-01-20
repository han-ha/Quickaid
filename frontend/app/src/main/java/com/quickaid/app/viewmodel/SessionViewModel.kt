package com.quickaid.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.datastore.SessionDataStore
import com.quickaid.app.data.api.UsersApi
import com.quickaid.app.enums.UserRole
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import retrofit2.Response
import javax.inject.Inject

@HiltViewModel
class SessionViewModel @Inject constructor(
    private val session: SessionDataStore,
    private val usersApi: UsersApi
) : ViewModel() {

    val role: StateFlow<UserRole> = session.role
        .map { it?.let { UserRole.fromString(it) } ?: UserRole.ANON }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), UserRole.ANON)

    val username: StateFlow<String?> = session.username
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), null)

    val darkModeEnabled: StateFlow<Boolean> = session.darkMode
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), false)

    val token: StateFlow<String?> = session.token
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), null)

    private val _deleteSuccess = MutableStateFlow(false)
    val deleteSuccess = _deleteSuccess.asStateFlow()

    fun setRole(role: UserRole) {
        viewModelScope.launch { session.setRole(role.toStorageString()) }
    }

    fun setUsername(name: String) {
        viewModelScope.launch { session.setUsername(name) }
    }

    fun setDarkMode(enabled: Boolean) {
        viewModelScope.launch { session.setDarkMode(enabled) }
    }

    fun logout() {
        viewModelScope.launch { session.clearSession() }
    }

    fun deleteAccount() {
        viewModelScope.launch {
            try {
                val response: Response<Unit> = usersApi.deleteMe()
                if (response.isSuccessful) {
                    session.clearSession()
                    _deleteSuccess.value = true
                } else {
                    println("Błąd usuwania: ${response.code()} ${response.message()}")
                }
            } catch (e: Exception) {
                println("Wyjątek usuwania: ${e.message}")
            }
        }
    }

    fun resetDeleteState() {
        _deleteSuccess.value = false
    }
}
