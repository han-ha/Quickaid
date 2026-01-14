package com.quickaid.app.ui.screens.quizzes

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.*
import com.quickaid.app.viewmodel.QuizDetailsViewModel

@Composable
fun QuizDetailsScreen(
    quizId: Int,
    navController: NavController,
    viewModel: QuizDetailsViewModel = hiltViewModel()
) {
    val quiz by viewModel.quiz.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val scrollState = rememberScrollState()

    val selectedAnswers = remember { mutableStateMapOf<Int, Int>() }
    var result by remember { mutableStateOf<Pair<Int, Int>?>(null) }
    var submitError by remember { mutableStateOf<String?>(null) }

    LaunchedEffect(quizId) {
        viewModel.fetchQuiz(quizId)
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSpacing.medium)
    ) {
        Box(
            modifier = Modifier
                .fillMaxWidth(),
            contentAlignment = Alignment.Center
        ) {
            Text(
                text = quiz?.title ?: "Szczegóły quizu",
                style = MaterialTheme.typography.headlineMedium
            )
        }

        Spacer(Modifier.height(AppSpacing.medium))

        when {
            isLoading -> Box(
                modifier = Modifier.fillMaxSize(),
                contentAlignment = Alignment.Center
            ) {
                CircularProgressIndicator()
            }

            error != null -> Text(
                "Błąd: $error",
                color = MaterialTheme.colorScheme.error
            )

            quiz != null -> {
                // tryb wynikowy
                if (result != null) {
                    val (points, maxPoints) = result!!

                    Column(
                        modifier = Modifier
                            .weight(1f)
                            .verticalScroll(scrollState)
                    ) {
                        quiz!!.questions.forEachIndexed { index, question ->
                            val correctAnswerId = question.answers.firstOrNull { it.isCorrect }?.id
                            val userAnswerId = selectedAnswers[question.id]

                            val questionCorrect = userAnswerId == correctAnswerId
                            val questionColor = if (questionCorrect) {
                                GreenPrimary.copy(alpha = 0.4f)
                            } else {
                                RedPrimary.copy(alpha = 0.4f)
                            }

                            Column(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .background(questionColor)
                                    .padding(vertical = AppSpacing.small)
                            ) {
                                Text(
                                    text = "${index + 1}. ${question.questionText}",
                                    style = MaterialTheme.typography.titleMedium
                                )

                                Spacer(Modifier.height(AppSpacing.extraSmall))

                                question.answers.forEach { answer ->
                                    val selected = userAnswerId == answer.id
                                    Row(
                                        modifier = Modifier
                                            .fillMaxWidth()
                                            .padding(vertical = AppSpacing.extraSmall),
                                        verticalAlignment = Alignment.CenterVertically
                                    ) {
                                        RadioButton(
                                            selected = selected,
                                            onClick = null
                                        )
                                        Spacer(Modifier.width(AppSpacing.small))
                                        Text(answer.answerText)
                                    }
                                }

                                Spacer(Modifier.height(AppSpacing.small))
                            }

                            Spacer(Modifier.height(AppSpacing.medium))
                        }
                    }

                    Spacer(Modifier.height(AppSpacing.medium))
                    Box(
                        modifier = Modifier.fillMaxWidth(),
                        contentAlignment = Alignment.Center
                    ) {
                        Text(
                            text = "Twój wynik: $points / $maxPoints",
                            style = MaterialTheme.typography.titleLarge
                        )
                    }
                    Box(
                        modifier = Modifier.fillMaxWidth(),
                        contentAlignment = Alignment.Center
                    ) {
                        LargeButton(
                            onClick = { navController.navigate("quizzes") },
                            modifier = Modifier.fillMaxWidth(),
                            content = "Powrót do listy quizów"
                        )

                    }
                }

                // tryb rozwiązywania
                else {
                    Column(
                        modifier = Modifier
                            .weight(1f)
                            .verticalScroll(scrollState)
                    ) {
                        quiz!!.questions.forEachIndexed { index, question ->
                            Text(
                                text = "${index + 1}. ${question.questionText}",
                                style = MaterialTheme.typography.titleMedium
                            )

                            Spacer(Modifier.height(AppSpacing.extraSmall))

                            question.answers.forEach { answer ->
                                val selected = selectedAnswers[question.id] == answer.id
                                Row(
                                    modifier = Modifier
                                        .fillMaxWidth()
                                        .clickable {
                                            selectedAnswers[question.id] = answer.id
                                        }
                                        .padding(vertical = AppSpacing.extraSmall),
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    RadioButton(
                                        selected = selected,
                                        onClick = { selectedAnswers[question.id] = answer.id }
                                    )
                                    Spacer(Modifier.width(AppSpacing.small))
                                    Text(answer.answerText)
                                }
                            }

                            Spacer(Modifier.height(AppSpacing.medium))
                        }
                    }

                    // komunikat błędu submitu
                    submitError?.let {
                        Spacer(Modifier.height(AppSpacing.small))
                        Text(it, color = MaterialTheme.colorScheme.error)
                    }

                    // przycisk sprawdzenia poprawności
                    LargeButton(
                        onClick = {
                            try {
                                viewModel.submitQuiz(
                                    quizId = quiz!!.id,
                                    answers = selectedAnswers
                                )

                                result = viewModel.calculateQuizPoints(
                                    quiz = quiz!!,
                                    selectedAnswers = selectedAnswers
                                )
                            } catch (e: Exception) {
                                submitError = "Nie udało się zapisać wyniku"
                            }
                        },
                        modifier = Modifier.fillMaxWidth(),
                        content = "Sprawdź poprawność"
                    )

                }
            }
        }
    }
}
