package com.quickaid.app.data.repository

import com.quickaid.app.data.api.AedApi
import com.quickaid.app.data.models.AedDto
import javax.inject.Inject

class AedRepository @Inject constructor(
    private val api: AedApi
) {

    suspend fun getAllAeds(): List<AedDto> =
        api.getAllAeds()

    suspend fun getAedById(id: Int): AedDto =
        api.getAedById(id)

    suspend fun addAed(aed: AedDto): AedDto =
        api.addAed(aed)

    suspend fun updateAed(id: Int, aed: AedDto): AedDto =
        api.updateAed(id, aed)

    suspend fun deleteAed(id: Int) =
        api.deleteAed(id)
}
