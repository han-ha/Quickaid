package com.quickaid.app.enums

// Enum reprezentujący rolę użytkownika
enum class UserRole {
    ANON, USER, ADMIN;

    companion object {
        fun fromString(rawRole: String?): UserRole = when (rawRole?.lowercase()) {
            "admin" -> ADMIN
            "user" -> USER
            else -> ANON
        }
    }

    fun toStorageString(): String = name.lowercase()
}