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
import com.quickaid.app.util.JwtUtils
import com.quickaid.app.viewmodel.ArticleViewModel
import com.quickaid.app.viewmodel.SessionViewModel

@Composable
fun AddArticleScreen(
    navController: NavController,
    viewModel: ArticleViewModel = hiltViewModel(),
    sessionViewModel: SessionViewModel = hiltViewModel(),
    jwtUtils: JwtUtils
) {
    var title by remember { mutableStateOf("") }
    var content by remember { mutableStateOf("") }

    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val addSuccess by viewModel.addSuccess.collectAsState()

    val token by sessionViewModel.token.collectAsState(initial = null)
    val userId = token?.let { jwtUtils.getUserId(it)?.toIntOrNull() } ?: 0
    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    LaunchedEffect(addSuccess) {
        if (addSuccess) {
            savedStateHandle?.set("articlesUpdated", true)
            navController.popBackStack()
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            "Dodaj artykuł",
            style = MaterialTheme.typography.headlineMedium,
            textAlign = TextAlign.Center)
        Spacer(Modifier.height(AppSizes.medium))

        OutlinedTextField(
            value = title,
            onValueChange = { title = it },
            label = { Text("Tytuł") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true
        )

        Spacer(Modifier.height(AppSizes.small))

        OutlinedTextField(
            value = content,
            onValueChange = { content = it },
            label = { Text("Treść") },
            modifier = Modifier
                .fillMaxWidth()
                .height(AppSizes.outlinedTextFieldHeightLarge)
        )

        Spacer(Modifier.height(AppSizes.medium))

        if (error != null) {
            Text(text = "Błąd: $error", color = MaterialTheme.colorScheme.error)
        }

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
