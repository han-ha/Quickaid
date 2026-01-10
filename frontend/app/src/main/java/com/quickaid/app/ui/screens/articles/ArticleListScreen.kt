package com.quickaid.app.ui.screens.articles

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.ArticleDto
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.AdminActions
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSpacing
import com.quickaid.app.viewmodel.ArticleViewModel
import com.quickaid.app.viewmodel.SessionViewModel

@Composable
fun ArticleListScreen(
    navController: NavController,
    viewModel: ArticleViewModel = hiltViewModel(),
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    val articles by viewModel.articles.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val role by sessionViewModel.role.collectAsState()

    var articleToDeleteId by remember { mutableStateOf<Int?>(null) }
    var articleToDeleteTitle by remember { mutableStateOf<String?>(null) }

    val savedStateHandle = navController.currentBackStackEntry?.savedStateHandle
    val articlesUpdated by savedStateHandle
        ?.getStateFlow("articlesUpdated", false)
        ?.collectAsState() ?: remember { mutableStateOf(false) }

    val deleteSuccess by viewModel.deleteSuccess.collectAsState()
    val addSuccess by viewModel.addSuccess.collectAsState()
    val updateSuccess by viewModel.updateSuccess.collectAsState()

    LaunchedEffect(Unit) {
        viewModel.fetchArticles()
    }

    LaunchedEffect(articlesUpdated) {
        if (articlesUpdated) {
            viewModel.fetchArticles()
            savedStateHandle?.set("articlesUpdated", false)
        }
    }

    LaunchedEffect(deleteSuccess) {
        if (deleteSuccess) {
            viewModel.fetchArticles()
            viewModel.resetDeleteState()
        }
    }

    LaunchedEffect(addSuccess) {
        if (addSuccess) {
            viewModel.fetchArticles()
            viewModel.resetAddState()
        }
    }

    LaunchedEffect(updateSuccess) {
        if (updateSuccess) {
            viewModel.fetchArticles()
            viewModel.resetUpdateState()
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSpacing.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = "Materiały edukacyjne",
            style = MaterialTheme.typography.headlineMedium
        )

        Spacer(Modifier.height(AppSpacing.large))

        when {
            isLoading -> CircularProgressIndicator()
            error != null -> Text(
                text = "Błąd: $error",
                color = MaterialTheme.colorScheme.error,
                style = MaterialTheme.typography.bodyMedium
            )
            else -> LazyColumn(
                modifier = Modifier.weight(1f),
                verticalArrangement = Arrangement.spacedBy(AppSpacing.small)
            ) {
                items(items = articles, key = { it.id }) { article ->
                    Card(
                        modifier = Modifier
                            .fillMaxWidth()
                            .clickable {
                                navController.navigate("articleDetails/${article.id}")
                            }
                    ) {
                        Column(modifier = Modifier.padding(AppSpacing.medium)) {
                            Text(
                                text = article.title,
                                style = MaterialTheme.typography.headlineSmall
                            )

                            if (role == UserRole.ADMIN) {
                                Spacer(Modifier.height(AppSpacing.small))
                                AdminActions(
                                    onEdit = {
                                        navController.navigate("editArticle/${article.id}")
                                    },
                                    onDelete = {
                                        articleToDeleteId = article.id
                                        articleToDeleteTitle = article.title
                                    }
                                )
                            }
                        }
                    }
                }
            }
        }

        if (role == UserRole.ADMIN) {
            Spacer(Modifier.height(AppSpacing.medium))
            LargeButton(
                onClick = { navController.navigate("addArticle") },
                modifier = Modifier.fillMaxWidth(),
                content = "Dodaj artykuł",
                buttonColor = MaterialTheme.colorScheme.primary
            )

        }

        if (articleToDeleteId != null) {
            AlertDialog(
                onDismissRequest = {
                    articleToDeleteId = null
                    articleToDeleteTitle = null
                },
                title = { Text("Potwierdzenie usunięcia") },
                text = {
                    Text(
                        "Czy na pewno chcesz usunąć artykuł \"${articleToDeleteTitle}\"? " +
                                "Ta operacja jest nieodwracalna."
                    )
                },
                confirmButton = {
                    SmallButton(
                        onClick = {
                            articleToDeleteId?.let {
                                viewModel.deleteArticle(it)
                            }
                            articleToDeleteId = null
                            articleToDeleteTitle = null
                        },
                        content = "Usuń",
                        buttonColor = MaterialTheme.colorScheme.error
                    )
                },
                dismissButton = {
                    SmallButton(
                        onClick = {
                            articleToDeleteId = null
                            articleToDeleteTitle = null
                        },
                        content = "Anuluj"
                    )

                }
            )
        }
    }
}
