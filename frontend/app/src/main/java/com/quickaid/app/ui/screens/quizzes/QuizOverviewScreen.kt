package com.quickaid.app.ui.screens.quizzes

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.*
import com.quickaid.app.viewmodel.QuizOverviewViewModel
import java.text.ParseException
import java.text.SimpleDateFormat
import java.util.*

@Composable
fun QuizOverviewScreen(
    quizId: Int,
    quizTitle: String,
    quizMaxScore: Int,
    navController: NavController,
    viewModel: QuizOverviewViewModel = hiltViewModel()
) {
    val bestResult by viewModel.bestResult.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()

    LaunchedEffect(quizId) {
        viewModel.fetchBestResult(quizId)
    }

    val formattedDate = remember(bestResult) {
        bestResult?.completedAt?.let { dateStr ->
            try {
                val parsed = try {
                    SimpleDateFormat(
                        "yyyy-MM-dd'T'HH:mm:ss.SSS",
                        Locale.getDefault()
                    ).parse(dateStr)
                } catch (e: ParseException) {
                    SimpleDateFormat(
                        "yyyy-MM-dd'T'HH:mm:ss",
                        Locale.getDefault()
                    ).parse(dateStr)
                }
                SimpleDateFormat(
                    "dd.MM.yyyy HH:mm",
                    Locale.getDefault()
                ).format(parsed!!)
            } catch (e: Exception) {
                dateStr
            }
        }
    }

    if (isLoading) {
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
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Top
    ) {
        Text(
            text = quizTitle,
            style = AppTypography.headlineMedium
        )

        Spacer(Modifier.height(AppSizes.large))

        when {
            error != null -> {
                Text(
                    text = "Błąd: $error",
                    color = MaterialTheme.colorScheme.error,
                    style = AppTypography.bodyMedium
                )
            }

            bestResult != null -> {
                Text(
                    text = "Twój najlepszy wynik",
                    style = AppTypography.bodyMedium
                )

                Text(
                    text = "${bestResult!!.score}/$quizMaxScore",
                    style = AppTypography.headlineMedium,
                    color = GreenPrimary,
                    modifier = Modifier.padding(top = AppSizes.small)
                )

                formattedDate?.let {
                    Text(
                        text = "Osiągnięto po raz pierwszy: $it",
                        style = AppTypography.bodyMedium,
                        modifier = Modifier.padding(top = AppSizes.small)
                    )
                }
            }

            else -> {
                Text(
                    text = "Jeszcze nie rozwiązałeś tego quizu.",
                    style = AppTypography.bodyMedium
                )


            }
        }
        Spacer(Modifier.height(AppSizes.large))

        LargeButton(
            onClick = { navController.navigate("quizDetails/$quizId") },
            modifier = Modifier.fillMaxWidth(),
            content = "Rozwiąż quiz"
        )

    }
}