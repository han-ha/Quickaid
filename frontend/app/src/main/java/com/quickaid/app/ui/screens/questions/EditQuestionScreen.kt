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
import com.quickaid.app.ui.theme.AppSpacing
import com.quickaid.app.viewmodel.QuestionViewModel

@Composable
fun EditQuestionScreen(
    questionId: Int?,
    quizId: Int,
    navController: NavController,
    questionViewModel: QuestionViewModel = hiltViewModel()
) {
    val isLoading by questionViewModel.isLoading.collectAsState()
    val saveSuccess by questionViewModel.saveSuccess.collectAsState()
    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    val question by questionViewModel.question.collectAsState()

    LaunchedEffect(questionId) {
        questionId?.let {
            questionViewModel.fetchQuestionById(it)
        }
    }

    LaunchedEffect(saveSuccess) {
        if (saveSuccess) {
            savedStateHandle?.set("quizUpdated", true)
            questionViewModel.resetSaveState()
            navController.popBackStack()
        }
    }

    Box(modifier = Modifier.fillMaxSize()) {
        question?.let { q ->
            QuestionForm(
                viewModel = questionViewModel,
                questionId = q.id,
                quizId = quizId,
                onSave = { updatedQuestion ->
                    questionViewModel.updateQuestion(q.id, updatedQuestion)
                }
            )
        }

        if (isLoading) {
            Spacer(modifier = Modifier.height(AppSpacing.medium))
            CircularProgressIndicator()
        }
    }
}
