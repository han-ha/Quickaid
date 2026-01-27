package com.quickaid.app.ui.screens.quizzes

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Card
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.AdminActions
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.QuizViewModel
import com.quickaid.app.viewmodel.SessionViewModel
import java.net.URLEncoder

@Composable
fun QuizListScreen(
    navController: NavController,
    viewModel: QuizViewModel = hiltViewModel(),
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    // Stany z ViewModelu
    val quizzes by viewModel.quizzes.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val role by sessionViewModel.role.collectAsState(initial = null)

    // Lokalny stan
    var quizToDeleteId by remember { mutableStateOf<Int?>(null) }
    var quizToDeleteTitle by remember { mutableStateOf<String?>(null) }

    val savedStateHandle = navController.currentBackStackEntry?.savedStateHandle

    // Pobranie quizów przy pierwszym wyświetleniu ekranu
    LaunchedEffect(Unit) { viewModel.fetchQuizzes() }

    // Stany sukcesów operacji
    val addSuccess by viewModel.addSuccess.collectAsState()
    val updateSuccess by viewModel.updateSuccess.collectAsState()
    val deleteSuccess by viewModel.deleteSuccess.collectAsState()

    // Reakcja na dodanie quizu - pobranie listy i reset flagi
    LaunchedEffect(addSuccess) {
        if (addSuccess) {
            viewModel.fetchQuizzes()
            viewModel.resetAddState()
        }
    }

    // Reakcja na aktualizację quizu - pobranie listy i reset flagi
    LaunchedEffect(updateSuccess) {
        if (updateSuccess) {
            viewModel.fetchQuizzes()
            viewModel.resetUpdateState()
        }
    }

    // Reakcja na usunięcie quizu: pobranie listy i reset flagi
    LaunchedEffect(deleteSuccess) {
        if (deleteSuccess) {
            viewModel.fetchQuizzes()
            viewModel.resetDeleteState()
        }
    }

    // Odbiór sygnału z poprzedniego ekranu, że quizy zostały zaktualizowane
    LaunchedEffect(savedStateHandle) {
        savedStateHandle?.getStateFlow("quizzesUpdated", false)?.collect { updated ->
            if (updated) {
                viewModel.fetchQuizzes()
                savedStateHandle["quizzesUpdated"] = false
            }
        }
    }

    Box(modifier = Modifier.fillMaxSize()) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(AppSizes.medium),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            // Nagłówek ekranu
            Text("Quizy", style = MaterialTheme.typography.headlineMedium)
            Spacer(Modifier.height(AppSizes.large))

            when {
                // Ekran ładowania
                isLoading -> Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator()
                }
                // Wyświetlenie błędu
                error != null -> Text(text = "Błąd: $error", color = MaterialTheme.colorScheme.error)
                else -> {
                    // Dla admina pokazujemy wszystkie quizy, dla zwykłego użytkownika tylko te z pytaniami
                    val visibleQuizzes = if (role == UserRole.ADMIN) quizzes
                    else quizzes.filter { it.numberOfQuestions > 0 }

                    LazyColumn(
                        modifier = Modifier.weight(1f),
                        verticalArrangement = Arrangement.spacedBy(AppSizes.small)
                    ) {
                        // Wyświetlenie listy quizów
                        items(visibleQuizzes, key = { it.id }) { quiz ->
                            Card(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .clickable {
                                        // Przejście do podglądu quizu
                                        val encodedTitle = URLEncoder.encode(quiz.title, "UTF-8")
                                        navController.navigate("quizOverview/${quiz.id}/$encodedTitle/${quiz.numberOfQuestions}")
                                    }
                            ) {
                                Column(modifier = Modifier.padding(AppSizes.medium)) {
                                    Text(quiz.title, style = MaterialTheme.typography.headlineSmall)

                                    // Opcje admina do edycji/usunięcia quizu
                                    if (role == UserRole.ADMIN) {
                                        Spacer(Modifier.height(AppSizes.small))
                                        AdminActions(
                                            onEdit = { navController.navigate("editQuiz/${quiz.id}") },
                                            onDelete = {
                                                quizToDeleteId = quiz.id
                                                quizToDeleteTitle = quiz.title
                                            }
                                        )
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Przycisk dodania nowego quizu
            if (role == UserRole.ADMIN) {
                Spacer(Modifier.height(AppSizes.medium))
                LargeButton(
                    onClick = { navController.navigate("addQuiz") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Dodaj quiz"
                )
            }

            // Dialog potwierdzający usunięcie quizu
            if (quizToDeleteId != null) {
                AlertDialog(
                    onDismissRequest = { quizToDeleteId = null; quizToDeleteTitle = null },
                    title = { Text("Potwierdzenie usunięcia") },
                    text = { Text("Czy na pewno chcesz usunąć quiz \"${quizToDeleteTitle}\"? Ta operacja jest nieodwracalna.") },
                    confirmButton = {
                        SmallButton(
                            onClick = {
                                quizToDeleteId?.let { viewModel.deleteQuiz(it) }
                                quizToDeleteId = null
                                quizToDeleteTitle = null
                            },
                            content = "Usuń",
                            buttonColor = MaterialTheme.colorScheme.error
                        )
                    },
                    dismissButton = {
                        SmallButton(
                            onClick = {
                                quizToDeleteId = null
                                quizToDeleteTitle = null
                            },
                            content = "Anuluj"
                        )
                    }
                )
            }
        }

        // Przycisk powrotu do ekranu głównego
        CustomIconButton(
            onClick = {
                navController.navigate("home") {
                    popUpTo(navController.graph.startDestinationId) { inclusive = true }
                }
            },
            modifier = Modifier
                .align(Alignment.TopEnd)
                .padding(AppSizes.medium)
                .size(AppSizes.extraLarge),
            icon = Icons.Filled.Home,
            contentDescription = "Powrót do ekranu głównego"
        )
    }
}
