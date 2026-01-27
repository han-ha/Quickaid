package com.quickaid.app.ui.screens.users

import android.util.Log
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.components.UserItem
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AdminViewModel

@Composable
fun UserListScreen(
    navController: NavController,
    onlyAdmins: Boolean = false,
    viewModel: AdminViewModel = hiltViewModel()
) {
    // Stany z ViewModelu
    val users by viewModel.users.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val currentUserId by viewModel.currentUserId.collectAsState()

    // Obsługa stanu powrotu z edycji użytkowników
    val savedStateHandle = navController.currentBackStackEntry?.savedStateHandle
    val usersUpdated by savedStateHandle
        ?.getStateFlow("usersUpdated", false)
        ?.collectAsState() ?: remember { mutableStateOf(false) }

    // Pobranie użytkowników przy uruchomieniu
    LaunchedEffect(Unit) {
        viewModel.fetchUsers()
    }

    // Pobranie użytkowników po aktualizacji
    LaunchedEffect(usersUpdated) {
        if (usersUpdated) {
            viewModel.fetchUsers()
            savedStateHandle?.set("usersUpdated", false)
        }
    }

    // Filtrowanie użytkowników, jeśli chcemy tylko administratorów
    val filteredUsers = users.filter { !onlyAdmins || it.role == "admin" }

    Box {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(AppSizes.medium),
            verticalArrangement = Arrangement.Top,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            // Nagłówek ekranu
            Text(
                text = if (onlyAdmins) "Administratorzy" else "Użytkownicy",
                style = MaterialTheme.typography.headlineMedium
            )
            Spacer(Modifier.height(AppSizes.medium))

            // Obsługa stanu ładowania, błędu i pustej listy
            when {
                isLoading -> CircularProgressIndicator()
                error != null -> {
                    Text(text = error ?: "", color = MaterialTheme.colorScheme.error)
                }

                // Wyświetlanie listy użytkowników
                filteredUsers.isEmpty() -> {
                    Text(
                        if (onlyAdmins) "Brak administratorów do wyświetlenia"
                        else "Brak użytkowników do wyświetlenia"
                    )
                }

                else -> LazyColumn(
                    modifier = Modifier.fillMaxSize(),
                    verticalArrangement = Arrangement.spacedBy(AppSizes.medium)
                ) {
                    items(filteredUsers, key = { it.id }) { user ->
                        UserItem(user, viewModel, navController, currentUserId)
                    }
                }
            }
        }

        // Przycisk powrotu do ekranu głównego
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
}
