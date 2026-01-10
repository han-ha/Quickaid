package com.quickaid.app.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSpacing
import com.quickaid.app.viewmodel.SessionViewModel
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.DarkModeRow

@Composable
fun SettingsScreen(
    navController: NavController,
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    val role by sessionViewModel.role.collectAsState()
    val darkModeEnabled by sessionViewModel.darkModeEnabled.collectAsState()
    val deleteSuccess by sessionViewModel.deleteSuccess.collectAsState()

    var showDeleteDialog by remember { mutableStateOf(false) }
    var showDeletedPopup by remember { mutableStateOf(false) }

    LaunchedEffect(deleteSuccess) {
        if (deleteSuccess) {
            showDeletedPopup = true
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSpacing.medium),
        verticalArrangement = Arrangement.Top,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = "Ustawienia",
            style = MaterialTheme.typography.headlineMedium
        )

        Spacer(Modifier.height(AppSpacing.large))

        DarkModeRow(
            darkModeEnabled = darkModeEnabled,
            onToggle = { enabled -> sessionViewModel.setDarkMode(enabled) }
        )

        Spacer(Modifier.height(AppSpacing.large))

        when (role) {
            UserRole.USER -> {
                SmallButton(
                    onClick = { navController.navigate("changePassword") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Zmień hasło"
                )

                Spacer(Modifier.height(AppSpacing.medium))

                SmallButton(
                    onClick = { showDeleteDialog = true },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Usuń konto"
                )
            }

            UserRole.ADMIN -> {
                SmallButton(
                    onClick = { navController.navigate("userManagement") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Zarządzanie użytkownikami"
                )

                Spacer(Modifier.height(AppSpacing.medium))

                SmallButton(
                    onClick = { navController.navigate("adminList") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Lista administratorów"
                )
            }

            UserRole.ANON -> { /* anon nie ma ustawień */ }
        }

        Spacer(Modifier.height(AppSpacing.extraLarge))

        SmallButton(
            onClick = {
                sessionViewModel.logout()
                navController.navigate("welcome") {
                    popUpTo("home") { inclusive = true }
                }
            },
            modifier = Modifier.fillMaxWidth(),
            content = "Wyloguj się"
        )
    }

    if (showDeleteDialog) {
        AlertDialog(
            onDismissRequest = { showDeleteDialog = false },
            title = { Text("Usuń konto") },
            text = { Text("Czy na pewno chcesz usunąć swoje konto? Tej operacji nie można cofnąć.") },
            confirmButton = {
                SmallButton(
                    onClick = {
                        showDeleteDialog = false
                        sessionViewModel.deleteAccount()
                    },
                    content = "Usuń",
                    buttonColor = MaterialTheme.colorScheme.error
                )
            },
            dismissButton = {
                SmallButton(
                    onClick = { showDeleteDialog = false },
                    content = "Anuluj"
                )
            }
        )
    }

    if (showDeletedPopup) {
        AlertDialog(
            onDismissRequest = {},
            title = { Text("Konto usunięte") },
            text = { Text("Twoje konto zostało pomyślnie usunięte.") },
            confirmButton = {
                SmallButton(
                    onClick = {
                        showDeletedPopup = false
                        sessionViewModel.resetDeleteState()
                        navController.navigate("welcome") {
                            popUpTo("home") { inclusive = true }
                        }
                    },
                    content = "Przejdź do ekranu głównego"
                )
            },
            dismissButton = null
        )
    }
}


