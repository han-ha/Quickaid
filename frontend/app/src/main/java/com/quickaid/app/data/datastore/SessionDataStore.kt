package com.quickaid.app.data.datastore

import android.content.Context
import androidx.datastore.preferences.preferencesDataStore
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.core.booleanPreferencesKey
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map

private val Context.sessionDataStore by preferencesDataStore("session")

class SessionDataStore(private val context: Context) {

    companion object {
        private val TOKEN_KEY = stringPreferencesKey("token")
        private val ROLE_KEY = stringPreferencesKey("role")
        private val USERNAME_KEY = stringPreferencesKey("username")
        private val USER_ID_KEY = stringPreferencesKey("user_id")
        private val DARK_MODE_KEY = booleanPreferencesKey("dark_mode")
    }

    val token: Flow<String?> = context.sessionDataStore.data.map { it[TOKEN_KEY] }
    val role: Flow<String?> = context.sessionDataStore.data.map { it[ROLE_KEY] }
    val username: Flow<String?> = context.sessionDataStore.data.map { it[USERNAME_KEY] }
    val userId: Flow<String?> = context.sessionDataStore.data.map { it[USER_ID_KEY] }
    val darkMode: Flow<Boolean> = context.sessionDataStore.data.map { it[DARK_MODE_KEY] ?: false }

    suspend fun getToken(): String? = token.first()

    suspend fun saveSession(
        token: String,
        role: String?,
        username: String?,
        userId: String?
    ) {
        context.sessionDataStore.edit {
            it[TOKEN_KEY] = token
            it[ROLE_KEY] = role ?: ""
            it[USERNAME_KEY] = username ?: ""
            it[USER_ID_KEY] = userId ?: ""
        }
    }

    suspend fun clearSession() {
        context.sessionDataStore.edit { it.clear() }
    }

    suspend fun setRole(role: String) {
        context.sessionDataStore.edit { it[ROLE_KEY] = role }
    }

    suspend fun setUsername(name: String) {
        context.sessionDataStore.edit { it[USERNAME_KEY] = name }
    }

    suspend fun setDarkMode(enabled: Boolean) {
        context.sessionDataStore.edit { it[DARK_MODE_KEY] = enabled }
    }
}
