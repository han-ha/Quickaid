package com.quickaid.app.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.hilt.navigation.compose.hiltViewModel
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSpacing
import com.quickaid.app.viewmodel.AuthViewModel

@Composable
fun RegisterScreen(
    viewModel: AuthViewModel = hiltViewModel(),
    onSuccess: () -> Unit
) {
    val authState by viewModel.authStateDto.collectAsState()

    var username by remember { mutableStateOf("") }
    var email by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var confirmPassword by remember { mutableStateOf("") }

    var showDialog by remember { mutableStateOf(false) }
    var dialogMessage by remember { mutableStateOf("") }
    var handledSuccess by remember { mutableStateOf(false) }

    LaunchedEffect(authState) {
        when (authState) {
            is AuthStateDto.Success -> {
                if (!handledSuccess) {
                    handledSuccess = true
                    dialogMessage = "Konto zostało utworzone pomyślnie!"
                    showDialog = true
                    onSuccess()
                }
            }
            is AuthStateDto.Error -> {
                dialogMessage = (authState as AuthStateDto.Error).message
                showDialog = true
            }
            else -> {}
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSpacing.medium),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {

        Text(
            text = "Zarejestruj się",
            style = MaterialTheme.typography.headlineMedium,
            modifier = Modifier.padding(bottom = AppSpacing.large)
        )

        TextField(
            value = username,
            onValueChange = { username = it },
            label = { Text("Nazwa użytkownika") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small
        )

        Spacer(Modifier.height(AppSpacing.small))

        TextField(
            value = email,
            onValueChange = { email = it },
            label = { Text("Adres e-mail") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small
        )

        Spacer(Modifier.height(AppSpacing.small))

        TextField(
            value = password,
            onValueChange = { password = it },
            label = { Text("Hasło") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small,
            visualTransformation = PasswordVisualTransformation()
        )

        Spacer(Modifier.height(AppSpacing.small))

        TextField(
            value = confirmPassword,
            onValueChange = { confirmPassword = it },
            label = { Text("Powtórz hasło") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small,
            visualTransformation = PasswordVisualTransformation()
        )

        Spacer(Modifier.height(AppSpacing.medium))

        LargeButton(
            onClick = { viewModel.register(username, email, password) },
            modifier = Modifier.fillMaxWidth(),
            content = "Utwórz konto"
        )

        Spacer(Modifier.height(AppSpacing.medium))

        when (authState) {
            is AuthStateDto.Loading -> {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(AppSpacing.extraLarge),
                    contentAlignment = Alignment.Center
                ) {
                    CircularProgressIndicator()
                }
            }
            else -> {}
        }
    }

    if (showDialog) {
        AlertDialog(
            onDismissRequest = { showDialog = false },
            title = { Text(if (authState is AuthStateDto.Success) "Sukces" else "Błąd") },
            text = { Text(dialogMessage) },
            confirmButton = {
                SmallButton(
                    onClick = { showDialog = false },
                    content = "OK",
                    modifier = Modifier
                )
            }
        )
    }
}
