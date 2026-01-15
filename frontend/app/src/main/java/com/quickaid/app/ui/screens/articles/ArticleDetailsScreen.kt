package com.quickaid.app.ui.screens.articles

import androidx.compose.foundation.layout.*
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextAlign
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavBackStackEntry
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.ArticleDetailsViewModel

@Composable
fun ArticleDetailsScreen(
    backStackEntry: NavBackStackEntry,
    viewModel: ArticleDetailsViewModel = hiltViewModel(backStackEntry)
) {
    val article by viewModel.article.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = article?.title ?: "Szczegóły artykułu",
            style = MaterialTheme.typography.headlineMedium,
            textAlign = TextAlign.Center
        )

        Spacer(Modifier.height(AppSizes.large))

        when {
            isLoading -> CircularProgressIndicator()
            error != null -> Text(
                text = "Błąd: $error",
                color = MaterialTheme.colorScheme.error,
                style = MaterialTheme.typography.bodyMedium
            )
            article != null -> {
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = AppSizes.medium)
                ) {
                    Text(
                        text = article!!.content,
                        style = MaterialTheme.typography.bodyMedium
                    )
                }
            }
            else -> Text(
                text = "Nie znaleziono artykułu.",
                style = MaterialTheme.typography.bodyMedium
            )
        }
    }
}
