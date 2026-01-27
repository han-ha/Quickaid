package com.quickaid.app.util

import android.util.Base64
import org.json.JSONObject
import javax.inject.Inject
import javax.inject.Singleton

// Klasa do decodowania JWT i pobierania id, roli i nazwy użytkownika
@Singleton
class JwtUtils @Inject constructor() {

    private fun decodePayload(token: String): JSONObject? {
        return try {
            val parts = token.split(".")
            if (parts.size != 3) return null

            val payload = parts[1]
            val decodedBytes = Base64.decode(payload, Base64.URL_SAFE)
            val json = String(decodedBytes, Charsets.UTF_8)

            JSONObject(json)
        } catch (e: Exception) {
            null
        }
    }

    fun getRole(token: String): String? {
        return decodePayload(token)
            ?.optString("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
    }

    fun getUsername(token: String): String? {
        return decodePayload(token)?.optString("sub")
    }

    fun getUserId(token: String): String? {
        return decodePayload(token)?.optString("id")
    }
}
