package com.quickaid.app.ui.components

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.navigation.NavController
import com.quickaid.app.data.models.AnswerDto
import com.quickaid.app.data.models.QuestionDto
import com.quickaid.app.ui.theme.AppSpacing
import com.quickaid.app.viewmodel.QuestionViewModel

@Composable
fun QuestionForm(
    navController: NavController,
    viewModel: QuestionViewModel,
    questionId: Int?,
    quizId: Int?,
    onSave: ((QuestionDto) -> Unit)? = null
) {
    val scrollState = rememberScrollState()

    var questionText by remember { mutableStateOf("") }
    val answers = remember { mutableStateListOf<AnswerDto>() }
    var answerToDeleteIndex by remember { mutableStateOf<Int?>(null) }

    val error by viewModel.error.collectAsState()
    val saveSuccess by viewModel.saveSuccess.collectAsState()

    LaunchedEffect(questionId) {
        if (questionId != null) {
            val q = viewModel.getQuestionById(questionId)
            questionText = q.questionText
            answers.clear()
            answers.addAll(q.answers)
        } else {
            if (answers.isEmpty()) {
                answers.add(AnswerDto(id = 0, answerText = "", isCorrect = false))
                answers.add(AnswerDto(id = 0, answerText = "", isCorrect = false))
            }
        }
    }

    LaunchedEffect(saveSuccess) {
        if (saveSuccess) navController.popBackStack()
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .verticalScroll(scrollState)
            .padding(AppSpacing.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = if (questionId == null) "Dodaj pytanie" else "Edytuj pytanie",
            style = MaterialTheme.typography.headlineMedium
        )

        Spacer(Modifier.height(AppSpacing.medium))

        OutlinedTextField(
            value = questionText,
            onValueChange = { questionText = it },
            label = { Text("Treść pytania") },
            modifier = Modifier.fillMaxWidth()
        )

        Spacer(Modifier.height(AppSpacing.medium))

        answers.forEachIndexed { index, answer ->
            Column(
                modifier = Modifier.fillMaxWidth(),
                verticalArrangement = Arrangement.spacedBy(AppSpacing.small)
            ) {
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    modifier = Modifier.fillMaxWidth()
                ) {
                    OutlinedTextField(
                        value = answer.answerText,
                        onValueChange = { answers[index] = answer.copy(answerText = it) },
                        label = { Text("Odpowiedź ${index + 1}") },
                        modifier = Modifier.weight(1f)
                    )

                    Spacer(Modifier.width(AppSpacing.small))

                    IconButton(onClick = { answerToDeleteIndex = index }) {
                        Icon(Icons.Default.Delete, contentDescription = "Usuń odpowiedź")
                    }
                }

                Row(verticalAlignment = Alignment.CenterVertically) {
                    Checkbox(
                        checked = answer.isCorrect,
                        onCheckedChange = { answers[index] = answer.copy(isCorrect = it) }
                    )
                    Text("Poprawna odpowiedź")
                }
            }

            Spacer(Modifier.height(AppSpacing.small))
        }

        Button(
            onClick = { answers.add(AnswerDto(id = 0, answerText = "", isCorrect = false)) },
            modifier = Modifier.fillMaxWidth()
        ) {
            Text("Dodaj odpowiedź")
        }

        Spacer(Modifier.height(AppSpacing.medium))

        if (!error.isNullOrBlank()) {
            Text(
                text = "Błąd: $error",
                color = MaterialTheme.colorScheme.error
            )
        }

        Button(
            onClick = {
                if (quizId == null && questionId == null) error("quizId nie może być null przy dodawaniu pytania")
                val q = QuestionDto(
                    id = questionId ?: 0,
                    quizId = quizId ?: 0,
                    questionText = questionText,
                    answers = answers.toList()
                )
                if (questionId == null) viewModel.addQuestion(q) else viewModel.updateQuestion(questionId, q)
                onSave?.invoke(q)
            },
            enabled = questionText.isNotBlank()
                    && answers.size >= 2
                    && answers.all { it.answerText.isNotBlank() }
                    && answers.count { it.isCorrect } >= 1,
            modifier = Modifier.fillMaxWidth()
        ) {
            Text("Zapisz")
        }
    }

    if (answerToDeleteIndex != null) {
        AlertDialog(
            onDismissRequest = { answerToDeleteIndex = null },
            title = { Text("Usuń odpowiedź") },
            text = { Text("Czy na pewno chcesz usunąć tę odpowiedź?") },
            confirmButton = {
                Button(
                    onClick = {
                        answers.removeAt(answerToDeleteIndex!!)
                        answerToDeleteIndex = null
                    },
                    colors = ButtonDefaults.buttonColors(containerColor = MaterialTheme.colorScheme.error)
                ) {
                    Text("Usuń")
                }
            },
            dismissButton = {
                Button(onClick = { answerToDeleteIndex = null }) {
                    Text("Anuluj")
                }
            }
        )
    }
}
