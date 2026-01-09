package com.quickaid.app.data.repository

import com.quickaid.app.data.api.ArticleApi
import com.quickaid.app.data.models.ArticleDto
import javax.inject.Inject

class ArticleRepository @Inject constructor(
    private val api: ArticleApi
) {
    suspend fun getArticles() = api.getArticles()

    suspend fun getArticleById(id: Int) = api.getArticleById(id)

    suspend fun addArticle(article: ArticleDto) = api.addArticle(article)

    suspend fun updateArticle(id: Int, article: ArticleDto) = api.updateArticle(id, article)

    suspend fun deleteArticle(id: Int) = api.deleteArticle(id)
}
