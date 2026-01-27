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
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AuthViewModel

@Composable
fun RegisterScreen(
    viewModel: AuthViewModel = hiltViewModel(),
    onSuccess: () -> Unit
) {
    // Stan z ViewModelu
    val authState by viewModel.authState.collectAsState()

    // Lokalne stany pól formularza
    var username by remember { mutableStateOf("") }
    var email by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var confirmPassword by remember { mutableStateOf("") }

    // Stany dla pop-upu i sukcesu rejestracji
    var showDialog by remember { mutableStateOf(false) }
    var dialogMessage by remember { mutableStateOf("") }
    var handledSuccess by remember { mutableStateOf(false) }

    // Obsługa wyników rejestracji
    LaunchedEffect(authState) {
        when (authState) {
            is AuthState.Success -> {
                if (!handledSuccess) {
                    handledSuccess = true
                    dialogMessage = "Konto zostało utworzone pomyślnie!"
                    showDialog = true
                }
            }
            is AuthState.Error -> {
                dialogMessage = (authState as AuthState.Error).message
                showDialog = true
            }
            else -> {}
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {

        // Nagłówek
        Text(
            text = "Zarejestruj się",
            style = MaterialTheme.typography.headlineMedium,
            modifier = Modifier.padding(bottom = AppSizes.large)
        )

        // Pole nazwy użytkownika
        TextField(
            value = username,
            onValueChange = { username = it },
            label = { Text("Nazwa użytkownika") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small
        )

        Spacer(Modifier.height(AppSizes.small))

        // Pole adresu email
        TextField(
            value = email,
            onValueChange = { email = it },
            label = { Text("Adres e-mail") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small
        )

        Spacer(Modifier.height(AppSizes.small))

        // Pole hasła z maskowaniem
        TextField(
            value = password,
            onValueChange = { password = it },
            label = { Text("Hasło") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small,
            visualTransformation = PasswordVisualTransformation()
        )

        Spacer(Modifier.height(AppSizes.small))

        // Pole potwierdzenia hasła
        TextField(
            value = confirmPassword,
            onValueChange = { confirmPassword = it },
            label = { Text("Powtórz hasło") },
            modifier = Modifier.fillMaxWidth(),
            singleLine = true,
            shape = MaterialTheme.shapes.small,
            visualTransformation = PasswordVisualTransformation()
        )

        Spacer(Modifier.height(AppSizes.medium))

        // Przycisk utworzenia konta
        LargeButton(
            onClick = { viewModel.register(username, email, password, confirmPassword) },
            modifier = Modifier.fillMaxWidth(),
            content = "Utwórz konto"
        )

        Spacer(Modifier.height(AppSizes.medium))

        // Wyświetlanie stanu ładowania
        when (authState) {
            is AuthState.Loading -> {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(AppSizes.extraLarge),
                    contentAlignment = Alignment.Center
                ) {
                    CircularProgressIndicator()
                }
            }
            else -> {}
        }
    }

    // Pop-up sukcesu lub błędu rejestracji
    if (showDialog) {
        AlertDialog(
            onDismissRequest = { showDialog = false },
            title = { Text(if (authState is AuthState.Success) "Sukces" else "Błąd") },
            text = { Text(dialogMessage) },
            confirmButton = {
                SmallButton(
                    onClick = {
                        showDialog = false
                        if (authState is AuthState.Success) {
                            onSuccess()
                        }
                    },
                    content = "OK"
                )
            }
        )
    }
}
