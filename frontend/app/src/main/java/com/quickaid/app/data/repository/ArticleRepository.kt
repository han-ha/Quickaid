package com.quickaid.app.data.repository

import com.quickaid.app.data.api.ArticleApi
import com.quickaid.app.data.models.ArticleDto
import javax.inject.Inject

// Repository do operacji na artykułach
class ArticleRepository @Inject constructor(
    private val api: ArticleApi
) {

    // Pobiera wszystkie artykuły
    suspend fun getArticles() = api.getArticles()

    // Pobiera artykuł po Id
    suspend fun getArticleById(id: Int) = api.getArticleById(id)

    // Dodaje nowy artykuł
    suspend fun addArticle(article: ArticleDto) = api.addArticle(article)

    // Aktualizuje artykuł po Id
    suspend fun updateArticle(id: Int, article: ArticleDto) = api.updateArticle(id, article)

    // Usuwa artykuł po Id
    suspend fun deleteArticle(id: Int) = api.deleteArticle(id)
}
