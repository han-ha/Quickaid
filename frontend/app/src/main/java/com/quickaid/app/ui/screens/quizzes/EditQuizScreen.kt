package com.quickaid.app.ui.screens.quizzes

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.QuestionDto
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.AdminActions
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.QuestionViewModel
import com.quickaid.app.viewmodel.QuizViewModel
import com.quickaid.app.viewmodel.SessionViewModel

@Composable
fun EditQuizScreen(
    navController: NavController,
    quizId: Int,
    quizViewModel: QuizViewModel = hiltViewModel(),
    questionViewModel: QuestionViewModel = hiltViewModel(),
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    val selectedQuiz by quizViewModel.selectedQuiz.collectAsState()
    val isLoading by quizViewModel.isLoading.collectAsState()
    val error by quizViewModel.error.collectAsState()
    val role by sessionViewModel.role.collectAsState()

    var title by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var questionToDelete by remember { mutableStateOf<QuestionDto?>(null) }

    val savedStateHandle = navController.currentBackStackEntry?.savedStateHandle

    val quizUpdated by savedStateHandle
        ?.getStateFlow("quizUpdated", false)
        ?.collectAsState() ?: remember { mutableStateOf(false) }

    LaunchedEffect(quizUpdated) {
        if (quizUpdated) {
            quizViewModel.fetchQuizById(quizId)
            savedStateHandle?.set("quizUpdated", false)
        }
    }

    LaunchedEffect(quizId) {
        quizViewModel.fetchQuizById(quizId)
    }

    LaunchedEffect(selectedQuiz) {
        selectedQuiz?.let {
            title = it.title
            description = it.description
        }
    }

    val questions by remember(selectedQuiz) { derivedStateOf { selectedQuiz?.questions ?: emptyList() } }

    if (isLoading && selectedQuiz == null) {
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
            .padding(AppSizes.medium)
    ) {
        Text("Edytuj quiz", style = MaterialTheme.typography.headlineMedium)
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
            value = description,
            onValueChange = { description = it },
            label = { Text("Opis") },
            modifier = Modifier
                .fillMaxWidth()
                .height(AppSizes.outlinedTextFieldHeightMedium)
        )

        Spacer(Modifier.height(AppSizes.medium))

        Text("Pytania:", style = MaterialTheme.typography.headlineSmall)
        Spacer(Modifier.height(AppSizes.small))

        LazyColumn(
            modifier = Modifier.weight(1f),
            verticalArrangement = Arrangement.spacedBy(AppSizes.small)
        ) {
            items(questions, key = { it.id }) { question ->
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(modifier = Modifier.padding(AppSizes.medium)) {
                        Text(question.questionText)
                        Spacer(Modifier.height(AppSizes.extraSmall))
                        Text(
                            "${question.answers.size} odpowiedzi",
                            style = MaterialTheme.typography.bodySmall
                        )
                        if (role == UserRole.ADMIN) {
                            Spacer(Modifier.height(AppSizes.small))
                            AdminActions(
                                onEdit = {
                                    navController.navigate("editQuestion/${question.id}/$quizId")
                                },
                                onDelete = { questionToDelete = question }
                            )
                        }
                    }
                }
            }
        }

        Spacer(Modifier.height(AppSizes.medium))

        LargeButton(
            onClick = { navController.navigate("addQuestion/$quizId") },
            modifier = Modifier.fillMaxWidth(),
            content = "Dodaj pytanie"
        )

        Spacer(Modifier.height(AppSizes.medium))

        if (!error.isNullOrBlank()) {
            Text("Błąd: $error", color = MaterialTheme.colorScheme.error)
            Spacer(Modifier.height(AppSizes.medium))
        }

        LargeButton(
            onClick = {
                quizViewModel.updateQuiz(
                    quizId,
                    QuizDto(
                        id = quizId,
                        title = title,
                        description = description,
                        numberOfQuestions = questions.size,
                        questions = questions
                    )
                )
                navController.navigate("quizzes") {
                    popUpTo(0)
                }
            },
            modifier = Modifier.fillMaxWidth(),
            content = "Zapisz quiz",
            enabled = !isLoading && title.isNotBlank() && description.isNotBlank() && questions.count() > 1
        )

    }

    if (questionToDelete != null) {
        AlertDialog(
            onDismissRequest = { questionToDelete = null },
            title = { Text("Potwierdzenie usunięcia") },
            text = {
                Text("Czy na pewno chcesz usunąć pytanie: \"${questionToDelete?.questionText}\"?")
            },
            confirmButton = {
                SmallButton(
                    onClick = {
                        questionToDelete?.let { q ->
                            questionViewModel.deleteQuestion(q.id, quizId)
                            val updatedQuestions = questions.filter { it.id != q.id }
                            quizViewModel.updateQuestions(updatedQuestions)
                        }
                        questionToDelete = null
                    },
                    content = "Usuń",
                    buttonColor = MaterialTheme.colorScheme.error
                )
            },
            dismissButton = {
                SmallButton(
                    onClick = { questionToDelete = null },
                    content = "Anuluj"
                )
            }
        )
    }
}
