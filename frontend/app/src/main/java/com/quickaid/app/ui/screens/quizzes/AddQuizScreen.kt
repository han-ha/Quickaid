package com.quickaid.app.ui.screens.quizzes

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.QuizDto
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.QuizViewModel

@Composable
fun AddQuizScreen(
    navController: NavController,
    viewModel: QuizViewModel = hiltViewModel()
) {
    // Lokalne stany formularza
    var title by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var numberOfQuestions by remember { mutableStateOf("0") }

    // Stany z ViewModelu
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()

    var newQuizId by remember { mutableStateOf<Int?>(null) }

    // Po dodaniu quizu - przejście do edycji nowo utworzonego quizu
    LaunchedEffect(newQuizId) {
        newQuizId?.let {
            navController.navigate("editQuiz/$it") {
                popUpTo("addQuiz") { inclusive = true }
            }
            newQuizId = null
        }
    }

    Column(
        modifier = Modifier.fillMaxSize().padding(AppSizes.medium)
    ) {
        // Nagłówek ekranu
        Text("Dodaj quiz", style = MaterialTheme.typography.headlineMedium)
        Spacer(Modifier.height(AppSizes.medium))

        // Pole tytułu quizu
        OutlinedTextField(
            value = title,
            onValueChange = { title = it },
            label = { Text("Tytuł") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true
        )

        Spacer(Modifier.height(AppSizes.small))

        // Pole opisu quizu
        OutlinedTextField(
            value = description,
            onValueChange = { description = it },
            label = { Text("Opis") },
            modifier = Modifier.fillMaxWidth().height(AppSizes.outlinedTextFieldHeightLarge)
        )

        Spacer(Modifier.height(AppSizes.medium))

        // Komunikat błędu
        if (error != null) Text(text = "Błąd: $error", color = MaterialTheme.colorScheme.error)

        // Przycisk dodawania quizu i przejścia do edycji
        LargeButton(
            onClick = {
                viewModel.addQuiz(
                    QuizDto(
                        id = 0,
                        title = title,
                        description = description,
                        numberOfQuestions = numberOfQuestions.toIntOrNull() ?: 0,
                        questions = emptyList()
                    )
                ) { createdId ->
                    newQuizId = createdId
                }
            },
            modifier = Modifier.fillMaxWidth(),
            content = "Dodaj i edytuj",
            enabled = !isLoading && title.isNotBlank() && description.isNotBlank()
        )
    }
}
