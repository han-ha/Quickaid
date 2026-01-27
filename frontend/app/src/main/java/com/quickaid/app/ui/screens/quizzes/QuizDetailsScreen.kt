package com.quickaid.app.ui.screens.quizzes

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.RadioButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateMapOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.ui.theme.GreenPrimary
import com.quickaid.app.ui.theme.RedPrimary
import com.quickaid.app.viewmodel.QuizDetailsViewModel

@Composable
fun QuizDetailsScreen(
    quizId: Int,
    navController: NavController,
    viewModel: QuizDetailsViewModel = hiltViewModel()
) {
    // Stan z ViewModelu
    val quiz by viewModel.quiz.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val scrollState = rememberScrollState()

    // Lokalny stan
    val selectedAnswers = remember { mutableStateMapOf<Int, Int>() }
    var result by remember { mutableStateOf<Pair<Int, Int>?>(null) }
    var submitError by remember { mutableStateOf<String?>(null) }

    // Pobranie quizu po wejściu na ekran
    LaunchedEffect(quizId) {
        viewModel.fetchQuiz(quizId)
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium)
    ) {
        // Nagłówek
        Box(
            modifier = Modifier.fillMaxWidth(),
            contentAlignment = Alignment.Center
        ) {
            Text(
                text = quiz?.title ?: "Szczegóły quizu",
                style = MaterialTheme.typography.headlineMedium
            )
        }

        Spacer(Modifier.height(AppSizes.medium))

        when {
            // Ekran ładowania
            isLoading -> Box(
                modifier = Modifier.fillMaxSize(),
                contentAlignment = Alignment.Center
            ) {
                CircularProgressIndicator()
            }

            // Wyświetlenie błędu
            error != null -> Text(
                "Błąd: $error",
                color = MaterialTheme.colorScheme.error
            )

            // Quiz załadowany
            quiz != null -> {
                // Tryb wynikowy
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
                                    .padding(vertical = AppSizes.small)
                            ) {
                                Text(
                                    text = "${index + 1}. ${question.questionText}",
                                    style = MaterialTheme.typography.titleMedium
                                )

                                Spacer(Modifier.height(AppSizes.extraSmall))

                                question.answers.forEach { answer ->
                                    val selected = userAnswerId == answer.id
                                    Row(
                                        modifier = Modifier
                                            .fillMaxWidth()
                                            .padding(vertical = AppSizes.extraSmall),
                                        verticalAlignment = Alignment.CenterVertically
                                    ) {
                                        RadioButton(
                                            selected = selected,
                                            onClick = null
                                        )
                                        Spacer(Modifier.width(AppSizes.small))
                                        Text(answer.answerText)
                                    }
                                }

                                Spacer(Modifier.height(AppSizes.small))
                            }

                            Spacer(Modifier.height(AppSizes.medium))
                        }
                    }

                    Spacer(Modifier.height(AppSizes.medium))
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
                } else {
                    // Tryb rozwiązywania quizu
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

                            Spacer(Modifier.height(AppSizes.extraSmall))

                            question.answers.forEach { answer ->
                                val selected = selectedAnswers[question.id] == answer.id
                                Row(
                                    modifier = Modifier
                                        .fillMaxWidth()
                                        .clickable {
                                            selectedAnswers[question.id] = answer.id
                                        }
                                        .padding(vertical = AppSizes.extraSmall),
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    RadioButton(
                                        selected = selected,
                                        onClick = { selectedAnswers[question.id] = answer.id }
                                    )
                                    Spacer(Modifier.width(AppSizes.small))
                                    Text(answer.answerText)
                                }
                            }

                            Spacer(Modifier.height(AppSizes.medium))
                        }
                    }

                    // Komunikat błędu submitu
                    submitError?.let {
                        Spacer(Modifier.height(AppSizes.small))
                        Text(it, color = MaterialTheme.colorScheme.error)
                    }

                    // Przycisk sprawdzenia poprawności
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
