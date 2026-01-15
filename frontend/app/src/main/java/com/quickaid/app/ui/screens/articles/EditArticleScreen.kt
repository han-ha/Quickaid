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
fun EditArticleScreen(
    navController: NavController,
    articleId: Int,
    viewModel: ArticleViewModel = hiltViewModel(),
    sessionViewModel: SessionViewModel = hiltViewModel(),
    jwtUtils: JwtUtils
) {
    val article by viewModel.selectedArticle.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val token by sessionViewModel.token.collectAsState(initial = null)

    val userId = token?.let { jwtUtils.getUserId(it)?.toIntOrNull() } ?: 0

    var title by remember { mutableStateOf("") }
    var content by remember { mutableStateOf("") }

    LaunchedEffect(articleId) {
        viewModel.fetchArticleById(articleId)
    }

    LaunchedEffect(article) {
        article?.let {
            title = it.title
            content = it.content
        }
    }

    if (isLoading && article == null) {
        Box(
            modifier = Modifier.fillMaxSize(),
            contentAlignment = Alignment.Center
        ) {
            CircularProgressIndicator()
        }
        return
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            "Edytuj artykuł",
            style = MaterialTheme.typography.headlineMedium,
            textAlign = TextAlign.Center
        )
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
