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
    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    QuestionForm(
        viewModel = questionViewModel,
        questionId = null,
        quizId = quizId,
        onSave = { newQuestion ->
            questionViewModel.addQuestionToQuiz(quizId, newQuestion) {
                savedStateHandle?.set("quizUpdated", true)
                navController.popBackStack()
            }
        }
    )
}
