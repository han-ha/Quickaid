package com.quickaid.app.util

object AedFormValidator {

    fun validateLatitude(value: String): String? {
        val lat = value.toDoubleOrNull() ?: return "Nieprawidłowy format"
        return if (lat in -90.0..90.0) null else "Zakres: -90 do 90"
    }

    fun validateLongitude(value: String): String? {
        val lon = value.toDoubleOrNull() ?: return "Nieprawidłowy format"
        return if (lon in -180.0..180.0) null else "Zakres: -180 do 180"
    }
}
