package com.quickaid.app.ui.screens.questions

import androidx.compose.runtime.Composable
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.QuestionForm
import com.quickaid.app.viewmodel.QuestionViewModel

@Composable
fun AddQuestionScreen(
    navController: NavController,
    quizId: Int,
    questionViewModel: QuestionViewModel = hiltViewModel()
) {
    // Uzyskanie dostępu do savedStateHandle poprzedniego ekranu
    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    // Wyświetlenie formularza pytania
    QuestionForm(
        viewModel = questionViewModel,
        questionId = null, // Dodawanie nowego pytania, czyli id jeszcze nie istnieje
        quizId = quizId,
        onSave = { newQuestion ->
            // Po zapisaniu pytania: dodanie do quizu, ustawienie flagi i powrót
            questionViewModel.addQuestionToQuiz(quizId, newQuestion) {
                savedStateHandle?.set("quizUpdated", true)
                navController.popBackStack()
            }
        }
    )
}
