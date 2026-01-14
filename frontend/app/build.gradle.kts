plugins {
    alias(libs.plugins.androidApplication)
    alias(libs.plugins.kotlinAndroid)
    alias(libs.plugins.ksp)
    alias(libs.plugins.hilt.android)
    kotlin("kapt")
}

android {
    namespace = "com.quickaid.app"
    compileSdk = 36 // TODO poprawić wersję do 8.0

    defaultConfig {
        applicationId = "com.quickaid.app"
        minSdk = 26
        targetSdk = 36
        versionCode = 1
        versionName = "1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }

    buildFeatures {
        compose = true
    }

    composeOptions {
        kotlinCompilerExtensionVersion = "1.5.8"
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
    kotlinOptions {
        jvmTarget = "17"
    }

    packaging.resources.excludes += "/META-INF/{AL2.0,LGPL2.1}"
}

dependencies {
    // AndroidX + Lifecycle
    implementation(libs.androidxCoreKtx)
    implementation(libs.lifecycleRuntimeKtx)
    implementation(libs.activityCompose)

    // Compose
    implementation(platform(libs.composeBom))
    implementation(libs.composeUi)
    implementation(libs.composeUiGraphics)
    implementation(libs.composeUiToolingPreview)
    implementation(libs.material3)
    implementation(libs.material)

    // Navigation
    implementation(libs.navigationFragmentKtx)
    implementation(libs.navigationUiKtx)

    // Retrofit + OkHttp
    implementation(libs.retrofit)
    implementation(libs.converterGson)
    implementation(libs.loggingInterceptor)

    // Hilt
    implementation(libs.hiltAndroid)
    kapt(libs.hiltCompiler)
    implementation(libs.hiltNavigationCompose)

    // Tests
    testImplementation(libs.junit)
    androidTestImplementation(libs.junitExt)
    androidTestImplementation(libs.espressoCore)
    androidTestImplementation(platform(libs.composeBom))
    androidTestImplementation(libs.composeUiTestJunit4)

    debugImplementation(libs.composeUiTooling)
    debugImplementation(libs.composeUiTestManifest)

    implementation("androidx.datastore:datastore-preferences:1.1.1")
    implementation("androidx.datastore:datastore-core:1.1.1")
}

kapt {
    correctErrorTypes = true
}
