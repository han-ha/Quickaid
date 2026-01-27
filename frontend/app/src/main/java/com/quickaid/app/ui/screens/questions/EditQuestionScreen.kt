package com.quickaid.app.ui.screens.questions

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.QuestionForm
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.QuestionViewModel

@Composable
fun EditQuestionScreen(
    questionId: Int?,
    quizId: Int,
    navController: NavController,
    questionViewModel: QuestionViewModel = hiltViewModel()
) {
    // Stany z ViewModelu
    val isLoading by questionViewModel.isLoading.collectAsState()
    val saveSuccess by questionViewModel.saveSuccess.collectAsState()
    val question by questionViewModel.question.collectAsState()

    // Uzyskanie dostępu do savedStateHandle poprzedniego ekranu
    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    // Pobranie danych pytania po ID
    LaunchedEffect(questionId) {
        questionId?.let {
            questionViewModel.fetchQuestionById(it)
        }
    }

    // Reakcja na zapisanie/aktualizację pytania
    LaunchedEffect(saveSuccess) {
        if (saveSuccess) {
            savedStateHandle?.set("quizUpdated", true)
            questionViewModel.resetSaveState()
            navController.popBackStack()
        }
    }

    Box(modifier = Modifier.fillMaxSize()) {
        // Formularz pytania, jeśli dane zostały pobrane
        question?.let { q ->
            QuestionForm(
                viewModel = questionViewModel,
                questionId = q.id,
                quizId = quizId,
                onSave = { updatedQuestion ->
                    // Aktualizacja pytania w ViewModelu
                    questionViewModel.updateQuestion(q.id, updatedQuestion)
                }
            )
        }

        // Ładowanie, jeśli trwa pobieranie lub zapis
        if (isLoading) {
            Spacer(modifier = Modifier.height(AppSizes.medium))
            CircularProgressIndicator()
        }
    }
}
