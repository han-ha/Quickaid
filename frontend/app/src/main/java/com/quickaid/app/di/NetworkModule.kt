package com.quickaid.app.di

import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.quickaid.app.Constants
import com.quickaid.app.data.api.AdminApi
import com.quickaid.app.data.api.AuthApi
import com.quickaid.app.data.api.ArticleApi
import com.quickaid.app.data.api.QuestionApi
import com.quickaid.app.data.api.QuizApi
import com.quickaid.app.data.api.ResultApi
import com.quickaid.app.data.api.UsersApi
import com.quickaid.app.data.datastore.SessionDataStore
import com.quickaid.app.data.repository.AuthRepository
import com.quickaid.app.data.repository.ArticleRepository
import com.quickaid.app.data.repository.QuizRepository
import com.quickaid.app.data.repository.ResultRepository
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import kotlinx.coroutines.runBlocking
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.inject.Singleton
import javax.net.ssl.*

@Module
@InstallIn(SingletonComponent::class)
object NetworkModule {

    @Provides
    @Singleton
    fun provideGson(): Gson = GsonBuilder().create()

    private fun getUnsafeOkHttpClient(
        tokenProvider: () -> String?
    ): OkHttpClient {
        val trustAllCerts = arrayOf<TrustManager>(
            object : X509TrustManager {
                override fun checkClientTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
                override fun checkServerTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
                override fun getAcceptedIssuers(): Array<X509Certificate> = emptyArray()
            }
        )

        val sslContext = SSLContext.getInstance("SSL")
        sslContext.init(null, trustAllCerts, SecureRandom())
        val sslSocketFactory = sslContext.socketFactory

        val logging = HttpLoggingInterceptor().apply {
            level = HttpLoggingInterceptor.Level.BODY
        }

        return OkHttpClient.Builder()
            .sslSocketFactory(sslSocketFactory, trustAllCerts[0] as X509TrustManager)
            .hostnameVerifier { _, _ -> true }
            .addInterceptor { chain ->
                val requestBuilder = chain.request().newBuilder()
                tokenProvider()?.let { token ->
                    requestBuilder.addHeader("Authorization", "Bearer $token")
                }
                chain.proceed(requestBuilder.build())
            }
            .addInterceptor(logging)
            .build()
    }

    @Provides
    @Singleton
    fun provideOkHttpClient(
        sessionDataStore: SessionDataStore
    ): OkHttpClient {

        val tokenProvider = {
            runBlocking {
                sessionDataStore.getToken()
            }
        }

        return getUnsafeOkHttpClient(tokenProvider)
    }

    @Provides
    @Singleton
    fun provideRetrofit(
        gson: Gson,
        client: OkHttpClient
    ): Retrofit =
        Retrofit.Builder()
            .baseUrl(Constants.BASE_URL_HTTP)
            .addConverterFactory(GsonConverterFactory.create(gson))
            .client(client)
            .build()

    @Provides
    @Singleton
    fun provideAuthApi(retrofit: Retrofit): AuthApi =
        retrofit.create(AuthApi::class.java)

    @Provides
    @Singleton
    fun provideAuthRepository(api: AuthApi): AuthRepository =
        AuthRepository(api)

    @Provides
    @Singleton
    fun provideArticleApi(retrofit: Retrofit): ArticleApi =
        retrofit.create(ArticleApi::class.java)

    @Provides
    @Singleton
    fun provideArticleRepository(api: ArticleApi): ArticleRepository =
        ArticleRepository(api)

    @Provides
    @Singleton
    fun provideQuizApi(retrofit: Retrofit): QuizApi =
        retrofit.create(QuizApi::class.java)

    @Provides
    @Singleton
    fun provideQuizRepository(api: QuizApi): QuizRepository =
        QuizRepository(api)

    @Provides
    @Singleton
    fun provideAdminApi(retrofit: Retrofit): AdminApi =
        retrofit.create(AdminApi::class.java)

    @Provides
    @Singleton
    fun provideUsersApi(retrofit: Retrofit): UsersApi =
        retrofit.create(UsersApi::class.java)

    @Provides
    fun provideResultApi(retrofit: Retrofit): ResultApi =
        retrofit.create(ResultApi::class.java)

    @Provides
    fun provideResultRepository(api: ResultApi): ResultRepository =
        ResultRepository(api)

    @Provides
    @Singleton
    fun provideQuestionApi(retrofit: Retrofit): QuestionApi =
        retrofit.create(QuestionApi::class.java)

    @Provides
    @Singleton
    fun provideQuestionRepository(api: QuestionApi) =
        com.quickaid.app.data.repository.QuestionRepository(api)

    @Provides
    @Singleton
    fun provideAedApi(retrofit: Retrofit): com.quickaid.app.data.api.AedApi =
        retrofit.create(com.quickaid.app.data.api.AedApi::class.java)

    @Provides
    @Singleton
    fun provideAedRepository(api: com.quickaid.app.data.api.AedApi): com.quickaid.app.data.repository.AedRepository =
        com.quickaid.app.data.repository.AedRepository(api)


}
