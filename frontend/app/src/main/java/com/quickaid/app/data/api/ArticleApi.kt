package com.quickaid.app.data.api

import com.quickaid.app.data.models.ArticleDto
import retrofit2.Response
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.Path
import retrofit2.http.Body
import retrofit2.http.POST
import retrofit2.http.PUT

// Interfejs API do operacji na artykułach
interface ArticleApi {

    // Pobiera wszystkie artykuły
    @GET("articles")
    suspend fun getArticles(): List<ArticleDto>

    // Pobiera artykuł po Id
    @GET("articles/{id}")
    suspend fun getArticleById(@Path("id") id: Int): ArticleDto

    // Dodaje nowy artykuł
    @POST("articles")
    suspend fun addArticle(@Body article: ArticleDto): ArticleDto

    // Aktualizuje artykuł po Id
    @PUT("articles/{id}")
    suspend fun updateArticle(@Path("id") id: Int, @Body article: ArticleDto): ArticleDto

    // Usuwa artykuł po Id
    @DELETE("articles/{id}")
    suspend fun deleteArticle(@Path("id") id: Int): Response<Unit>
}
