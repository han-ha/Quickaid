package com.quickaid.app.util

object AedFormValidator {

    fun validateLatitude(value: String): Boolean {
        val lat = value.toDoubleOrNull() ?: return false
        return (lat in -90.0..90.0)
    }

    fun validateLongitude(value: String): Boolean {
        val lon = value.toDoubleOrNull() ?: return false
        return (lon in -180.0..180.0)
    }
}
