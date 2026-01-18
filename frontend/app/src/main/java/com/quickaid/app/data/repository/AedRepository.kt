package com.quickaid.app.data.repository

import com.quickaid.app.data.api.AedApi
import com.quickaid.app.data.models.AedDto
import javax.inject.Inject

class AedRepository @Inject constructor(
    private val api: AedApi
) {
    suspend fun getAllAeds(): List<AedDto> = api.getAllAeds()

    suspend fun saveAed(aed: AedDto): AedDto {
        return api.saveAed(aed)
    }
}
