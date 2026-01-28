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
import com.quickaid.app.data.models.AnswerDto
import com.quickaid.app.data.models.QuestionDto
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.QuestionViewModel

// Komponent formularza dodawania/edycji pytania
@Composable
fun QuestionForm(
    viewModel: QuestionViewModel,
    questionId: Int?,
    quizId: Int?,
    onSave: ((QuestionDto) -> Unit)? = null
) {
    val scrollState = rememberScrollState()
    val question by viewModel.question.collectAsState()
    val error by viewModel.error.collectAsState()

    val localAnswers = remember(question) {
        question?.answers?.toMutableStateList() ?: mutableStateListOf(
            AnswerDto(id = 0, answerText = "", isCorrect = false),
            AnswerDto(id = 0, answerText = "", isCorrect = false)
        )
    }

    var localQuestionText by remember(question) {
        mutableStateOf(question?.questionText ?: "")
    }

    var localCorrectAnswerIndex by remember(question) {
        mutableStateOf(question?.answers?.indexOfFirst { it.isCorrect }?.takeIf { it >= 0 })
    }

    var answerToDeleteIndex by remember { mutableStateOf<Int?>(null) }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .verticalScroll(scrollState)
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = if (questionId == null) "Dodaj pytanie" else "Edytuj pytanie",
            style = MaterialTheme.typography.headlineMedium
        )
        Spacer(Modifier.height(AppSizes.medium))
        OutlinedTextField(
            value = localQuestionText,
            onValueChange = { localQuestionText = it },
            label = { Text("Treść pytania") },
            modifier = Modifier.fillMaxWidth()
        )
        Spacer(Modifier.height(AppSizes.medium))
        localAnswers.forEachIndexed { index, answer ->
            Column(
                modifier = Modifier.fillMaxWidth(),
                verticalArrangement = Arrangement.spacedBy(AppSizes.small)
            ) {
                Row(verticalAlignment = Alignment.CenterVertically, modifier = Modifier.fillMaxWidth()) {
                    OutlinedTextField(
                        value = answer.answerText,
                        onValueChange = { localAnswers[index] = answer.copy(answerText = it) },
                        label = { Text("Odpowiedź ${index + 1}") },
                        modifier = Modifier.weight(1f)
                    )
                    Spacer(Modifier.width(AppSizes.small))
                    IconButton(onClick = { answerToDeleteIndex = index }) {
                        Icon(Icons.Default.Delete, contentDescription = "Usuń odpowiedź")
                    }
                }
                Row(verticalAlignment = Alignment.CenterVertically) {
                    RadioButton(
                        selected = localCorrectAnswerIndex == index,
                        onClick = { localCorrectAnswerIndex = index }
                    )
                    Text("Poprawna odpowiedź")
                }
            }
            Spacer(Modifier.height(AppSizes.small))
        }
        LargeButton(
            onClick = { localAnswers.add(AnswerDto(id = 0, answerText = "", isCorrect = false)) },
            modifier = Modifier.fillMaxWidth(),
            content = "Dodaj odpowiedź"
        )
        Spacer(Modifier.height(AppSizes.medium))
        if (!error.isNullOrBlank()) {
            Text(text = "Błąd: $error", color = MaterialTheme.colorScheme.error)
        }
        LargeButton(
            onClick = {
                val q = QuestionDto(
                    id = questionId ?: 0,
                    quizId = quizId ?: 0,
                    questionText = localQuestionText,
                    answers = localAnswers.mapIndexed { i, ans -> ans.copy(isCorrect = i == localCorrectAnswerIndex) }
                )
                if (questionId == null) viewModel.addQuestion(q) else viewModel.updateQuestion(questionId, q)
                onSave?.invoke(q)
            },
            enabled = localQuestionText.isNotBlank()
                    && localAnswers.size >= 2
                    && localAnswers.all { it.answerText.isNotBlank() }
                    && localCorrectAnswerIndex != null,
            modifier = Modifier.fillMaxWidth(),
            content = "Zapisz"
        )
    }

    if (answerToDeleteIndex != null) {
        AlertDialog(
            onDismissRequest = { answerToDeleteIndex = null },
            title = { Text("Potwierdzenie usunięcia") },
            text = { Text("Czy na pewno chcesz usunąć tę odpowiedź?") },
            confirmButton = {
                SmallButton(
                    onClick = {
                        localAnswers.removeAt(answerToDeleteIndex!!)
                        if (localCorrectAnswerIndex == answerToDeleteIndex) localCorrectAnswerIndex = null
                        else if (localCorrectAnswerIndex != null && localCorrectAnswerIndex!! > answerToDeleteIndex!!)
                            localCorrectAnswerIndex = localCorrectAnswerIndex!! - 1
                        answerToDeleteIndex = null
                    },
                    content = "Usuń",
                    buttonColor = MaterialTheme.colorScheme.error
                )
            },
            dismissButton = {
                SmallButton(
                    onClick = { answerToDeleteIndex = null },
                    content = "Anuluj"
                )
            }
        )
    }
}
