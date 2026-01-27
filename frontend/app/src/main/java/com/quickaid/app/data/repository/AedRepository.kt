package com.quickaid.app.data.repository

import com.quickaid.app.data.api.AedApi
import com.quickaid.app.data.models.AedDto
import javax.inject.Inject

// Repository do operacji na AED
class AedRepository @Inject constructor(
    private val api: AedApi
) {

    // Pobiera wszystkie AED
    suspend fun getAllAeds(): List<AedDto> =
        api.getAllAeds()

    // Pobiera AED po Id
    suspend fun getAedById(id: Int): AedDto =
        api.getAedById(id)

    // Dodaje nowe AED
    suspend fun addAed(aed: AedDto): AedDto =
        api.addAed(aed)

    // Aktualizuje AED po Id
    suspend fun updateAed(id: Int, aed: AedDto): AedDto =
        api.updateAed(id, aed)

    // Usuwa AED po Id
    suspend fun deleteAed(id: Int) =
        api.deleteAed(id)
}
