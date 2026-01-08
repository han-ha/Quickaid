package com.quickaid.app.data.api

import com.quickaid.app.data.models.ArticleDto
import retrofit2.Response
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.Path
import retrofit2.http.Body
import retrofit2.http.POST
import retrofit2.http.PUT

interface ArticleApi {

    @GET("articles")
    suspend fun getArticles(): List<ArticleDto>

    @GET("articles/{id}")
    suspend fun getArticleById(@Path("id") id: Int): ArticleDto

    @POST("articles")
    suspend fun addArticle(@Body article: ArticleDto): ArticleDto

    @PUT("articles/{id}")
    suspend fun updateArticle(@Path("id") id: Int, @Body article: ArticleDto): ArticleDto

    @DELETE("articles/{id}")
    suspend fun deleteArticle(@Path("id") id: Int): Response<Unit>

}
