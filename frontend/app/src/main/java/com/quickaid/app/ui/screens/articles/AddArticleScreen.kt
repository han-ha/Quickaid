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
fun AddArticleScreen(
    navController: NavController,
    viewModel: ArticleViewModel = hiltViewModel()
) {
    // Lokalne stany formularza
    var title by remember { mutableStateOf("") }
    var content by remember { mutableStateOf("") }

    // Stany z ViewModelu
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val addSuccess by viewModel.addSuccess.collectAsState()

    // Dostęp do savedStateHandle poprzedniego ekranu (do odświeżenia listy artykułów)
    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    // Reakcja na poprawne dodanie artykułu - cofnięcie ekranu i ustawienie flagi odświeżenia
    LaunchedEffect(addSuccess) {
        if (addSuccess) {
            savedStateHandle?.set("articlesUpdated", true)
            navController.popBackStack()
        }
    }

    // Główny kod ekranu z formularzem dodawania artykułu
    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        // Nagłówek ekranu
        Text(
            "Dodaj artykuł",
            style = MaterialTheme.typography.headlineMedium,
            textAlign = TextAlign.Center
        )
        Spacer(Modifier.height(AppSizes.medium))

        // Pole do wpisania tytułu artykułu
        OutlinedTextField(
            value = title,
            onValueChange = { title = it },
            label = { Text("Tytuł") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true
        )

        Spacer(Modifier.height(AppSizes.small))

        // Pole do wpisania treści artykułu
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

        // Przycisk dodania artykułu z walidacją i obsługą stanu ładowania
        LargeButton(
            onClick = {
                viewModel.addArticle(
                    ArticleDto(
                        id = 0,
                        title = title,
                        content = content
                    )
                )
            },
            modifier = Modifier.fillMaxWidth(),
            content = "Dodaj",
            buttonColor = MaterialTheme.colorScheme.primary,
            enabled = !isLoading && title.isNotBlank() && content.isNotBlank()
        )
    }
}
