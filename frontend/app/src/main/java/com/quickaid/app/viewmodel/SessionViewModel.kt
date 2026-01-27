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

    // Aktualna rola użytkownika z DataStore, jeśli nie ma zapisanej roli, przyjmujemy ANON
    val role: StateFlow<UserRole> = session.role
        .map { it?.let { UserRole.fromString(it) } ?: UserRole.ANON }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), UserRole.ANON)

    // Nazwa użytkownika z DataStore
    val username: StateFlow<String?> = session.username
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), null)

    // Informacja o włączeniu trybu ciemnego
    val darkModeEnabled: StateFlow<Boolean> = session.darkMode
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), false)

    // Token użytkownika
    val token: StateFlow<String?> = session.token
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000), null)

    // Flaga sukcesu usunięcia konta
    private val _deleteSuccess = MutableStateFlow(false)
    val deleteSuccess = _deleteSuccess.asStateFlow()

    // Zapisuje rolę użytkownika w DataStore
    fun setRole(role: UserRole) {
        viewModelScope.launch { session.setRole(role.toStorageString()) }
    }

    // Zapisuje nazwę użytkownika w DataStore
    fun setUsername(name: String) {
        viewModelScope.launch { session.setUsername(name) }
    }

    // Ustawia tryb ciemny w DataStore
    fun setDarkMode(enabled: Boolean) {
        viewModelScope.launch { session.setDarkMode(enabled) }
    }

    // Wylogowanie - czyści wszystkie dane DataStore
    fun logout() {
        viewModelScope.launch { session.clearSession() }
    }

    // Usuwa konto użytkownika
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

    // Resetuje flagę sukcesu usunięcia konta
    fun resetDeleteState() {
        _deleteSuccess.value = false
    }
}
