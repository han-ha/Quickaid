package com.quickaid.app.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.hilt.navigation.compose.hiltViewModel
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AuthViewModel

@Composable
fun LoginScreen(
    viewModel: AuthViewModel = hiltViewModel(),
    onSuccess: () -> Unit
) {
    val authState by viewModel.authStateDto.collectAsState()

    var username by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var handledSuccess by remember { mutableStateOf(false) }

    LaunchedEffect(authState) {
        if (authState is AuthStateDto.Success && !handledSuccess) {
            handledSuccess = true
            onSuccess()
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {

        Text(
            text = "Zaloguj się",
            style = MaterialTheme.typography.headlineMedium,
            modifier = Modifier.padding(bottom = AppSizes.large)
        )

        TextField(
            value = username,
            onValueChange = { username = it },
            label = { Text("Nazwa użytkownika") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small
        )

        Spacer(Modifier.height(AppSizes.small))

        TextField(
            value = password,
            onValueChange = { password = it },
            label = { Text("Hasło") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small,
            visualTransformation = PasswordVisualTransformation()
        )

        Spacer(Modifier.height(AppSizes.medium))

        LargeButton(
            onClick = { viewModel.login(username, password) },
            modifier = Modifier.fillMaxWidth(),
            content = "Zaloguj"
        )

        Spacer(Modifier.height(AppSizes.medium))

        when (authState) {
            is AuthStateDto.Loading -> {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(AppSizes.extraLarge),
                    contentAlignment = Alignment.Center
                ) {
                    CircularProgressIndicator()
                }
            }
            is AuthStateDto.Error -> {
                val message = (authState as? AuthStateDto.Error)?.message ?: "Nieznany błąd"
                Text(
                    text = "Błąd: $message",
                    color = MaterialTheme.colorScheme.error,
                    style = MaterialTheme.typography.bodyMedium
                )
            }
            else -> {}
        }
    }
}
