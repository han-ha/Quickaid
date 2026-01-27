package com.quickaid.app.ui.screens.articles

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextAlign
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.ArticleDto
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.ArticleViewModel

@Composable
fun EditArticleScreen(
    navController: NavController,
    articleId: Int,
    viewModel: ArticleViewModel = hiltViewModel()
) {
    // Stan z ViewModelu
    val article by viewModel.selectedArticle.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()

    // Lokalny stan formularza
    var title by remember { mutableStateOf("") }
    var content by remember { mutableStateOf("") }

    // Pobranie artykułu po jego ID przy wejściu na ekran
    LaunchedEffect(articleId) {
        viewModel.fetchArticleById(articleId)
    }

    // Aktualizacja lokalnych pól formularza po załadowaniu artykułu
    LaunchedEffect(article) {
        article?.let {
            title = it.title
            content = it.content
        }
    }

    // Ładowanie przy czekaniu na artykuł
    if (isLoading && article == null) {
        Box(
            modifier = Modifier.fillMaxSize(),
            contentAlignment = Alignment.Center
        ) {
            CircularProgressIndicator()
        }
        return
    }

    // Główny kod ekranu z formularzem edycji
    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        // Nagłówek ekranu
        Text(
            "Edytuj artykuł",
            style = MaterialTheme.typography.headlineMedium,
            textAlign = TextAlign.Center
        )
        Spacer(Modifier.height(AppSizes.medium))

        // Pole do edycji tytułu artykułu
        OutlinedTextField(
            value = title,
            onValueChange = { title = it },
            label = { Text("Tytuł") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true
        )

        Spacer(Modifier.height(AppSizes.small))

        // Pole do edycji treści artykułu
        OutlinedTextField(
            value = content,
            onValueChange = { content = it },
            label = { Text("Treść") },
            modifier = Modifier
                .fillMaxWidth()
                .height(AppSizes.outlinedTextFieldHeightLarge)
        )

        Spacer(Modifier.height(AppSizes.medium))

        // Wyświetlenie komunikatu błędu, jeśli wystąpił
        if (error != null) {
            Text(text = "Błąd: $error", color = MaterialTheme.colorScheme.error)
        }

        // Przycisk zapisu zmian artykułu (z odświeżaniem ekranu listy)
        LargeButton(
            onClick = {
                viewModel.updateArticle(
                    articleId,
                    ArticleDto(
                        id = articleId,
                        title = title,
                        content = content
                    )
                )
                navController.previousBackStackEntry
                    ?.savedStateHandle
                    ?.set("articlesUpdated", true)
                navController.popBackStack()
            },
            modifier = Modifier.fillMaxWidth(),
            content = "Zapisz",
            buttonColor = MaterialTheme.colorScheme.primary,
            enabled = !isLoading && title.isNotBlank() && content.isNotBlank()
        )

    }
}
