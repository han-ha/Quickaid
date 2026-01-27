package com.quickaid.app.data.models

import com.quickaid.app.enums.AedType

// DTO dla AED
data class AedDto(
    val id: Int?, // id w bazie
    val externalId: Long?, // id zewnętrzne
    val latitude: Double,
    val longitude: Double,
    val description: String?,
    val verified: Boolean,
    val type: AedType
)
