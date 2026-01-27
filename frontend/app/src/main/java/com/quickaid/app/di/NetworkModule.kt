package com.quickaid.app.di

import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.quickaid.app.Constants
import com.quickaid.app.data.api.*
import com.quickaid.app.data.datastore.SessionDataStore
import com.quickaid.app.data.repository.*
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
import java.util.concurrent.TimeUnit
import javax.inject.Singleton
import javax.net.ssl.*

@Module
@InstallIn(SingletonComponent::class)
object NetworkModule {

    // Dostarcza Gson do serializacji/deserializacji JSON
    @Provides
    @Singleton
    fun provideGson(): Gson = GsonBuilder().create()

    // Tworzy klienta OkHttp ignorującego certyfikaty SSL
    // (Uwaga: należy używać tylko w wersji deweloperskiej aplikacji!)
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

    // Dostarcza klienta HTTP z timeoutami i tokenem sesyjnym
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

        // W wersji deweloperskiej użyta jest funkcja ignorująca certyfikaty SSL
        return getUnsafeOkHttpClient(tokenProvider)
            .newBuilder()
            .connectTimeout(60, TimeUnit.SECONDS)
            .readTimeout(60, TimeUnit.SECONDS)
            .writeTimeout(60, TimeUnit.SECONDS)
            .build()
    }

    // Dostarcza Retrofit skonfigurowany z Gson i OkHttpClient
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

    // Dostarczenie API i repozytoriów

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
        QuestionRepository(api)

    @Provides
    @Singleton
    fun provideAedApi(retrofit: Retrofit): AedApi =
        retrofit.create(AedApi::class.java)

    @Provides
    @Singleton
    fun provideAedRepository(api: AedApi): AedRepository =
        AedRepository(api)
}
