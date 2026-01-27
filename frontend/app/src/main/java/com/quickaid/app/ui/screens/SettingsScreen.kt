package com.quickaid.app.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.SessionViewModel
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.components.DarkModeRow

@Composable
fun SettingsScreen(
    navController: NavController,
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    // Stan z ViewModelu
    val role by sessionViewModel.role.collectAsState()
    val darkModeEnabled by sessionViewModel.darkModeEnabled.collectAsState()
    val deleteSuccess by sessionViewModel.deleteSuccess.collectAsState()

    // Lokalny stan
    var showDeleteDialog by remember { mutableStateOf(false) }
    var showDeletedPopup by remember { mutableStateOf(false) }

    // Pokazanie pop-upu po pomyślnym usunięciu konta
    LaunchedEffect(deleteSuccess) {
        if (deleteSuccess) {
            showDeletedPopup = true
        }
    }

    Box {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(AppSizes.medium),
            verticalArrangement = Arrangement.Top,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            // Nagłówek
            Text(
                text = "Ustawienia",
                style = MaterialTheme.typography.headlineMedium
            )

            Spacer(Modifier.height(AppSizes.large))

            // Przełącznik trybu ciemnego
            DarkModeRow(
                darkModeEnabled = darkModeEnabled,
                onToggle = { enabled -> sessionViewModel.setDarkMode(enabled) }
            )

            Spacer(Modifier.height(AppSizes.small))

            // Opcje zależne od roli użytkownika
            when (role) {
                UserRole.USER -> {
                    // Usuwanie konta
                    SmallButton(
                        onClick = { showDeleteDialog = true },
                        modifier = Modifier.fillMaxWidth(),
                        content = "Usuń konto"
                    )
                }

                UserRole.ADMIN -> {
                    // Zarządzanie użytkownikami
                    SmallButton(
                        onClick = { navController.navigate("userManagement") },
                        modifier = Modifier.fillMaxWidth(),
                        content = "Zarządzanie użytkownikami"
                    )

                    Spacer(Modifier.height(AppSizes.small))

                    // Zarządzanie administratorami
                    SmallButton(
                        onClick = { navController.navigate("adminList") },
                        modifier = Modifier.fillMaxWidth(),
                        content = "Lista administratorów"
                    )
                }

                // Anonimowy użytkownik nie ma ustawień
                UserRole.ANON -> { }
            }

            Spacer(Modifier.height(AppSizes.small))

            // Przycisk wylogowania
            SmallButton(
                onClick = {
                    sessionViewModel.logout()
                    navController.navigate("welcome") {
                        popUpTo(0) { inclusive = true }
                        launchSingleTop = true
                    }

                },
                modifier = Modifier.fillMaxWidth(),
                content = "Wyloguj"
            )
        }

        // Przyciski powrotu do ekranu głównego
        CustomIconButton(
            onClick = {
                navController.navigate("home") {
                    popUpTo(navController.graph.startDestinationId) { inclusive = true }
                }
            },
            modifier = Modifier
                .align(Alignment.TopEnd)
                .padding(AppSizes.medium)
                .size(AppSizes.extraLarge),
            icon = Icons.Filled.Home,
            contentDescription = "Powrót do ekranu głównego"
        )
    }

    // Dialog potwierdzający usunięcie konta
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

    // Pop-up informujący o pomyślnym usunięciu konta
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
