package com.quickaid.app.ui.screens.users

import android.util.Log
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.UserItem
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AdminViewModel

@Composable
fun UserListScreen(
    navController: NavController,
    onlyAdmins: Boolean = false,
    viewModel: AdminViewModel = hiltViewModel()
) {
    val users by viewModel.users.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val currentUserId by viewModel.currentUserId.collectAsState()

    val savedStateHandle = navController.currentBackStackEntry?.savedStateHandle
    val usersUpdated by savedStateHandle
        ?.getStateFlow("usersUpdated", false)
        ?.collectAsState() ?: remember { mutableStateOf(false) }

    LaunchedEffect(Unit) {
        viewModel.fetchUsers()
    }

    LaunchedEffect(usersUpdated) {
        if (usersUpdated) {
            viewModel.fetchUsers()
            savedStateHandle?.set("usersUpdated", false)
        }
    }

    val filteredUsers = users.filter { !onlyAdmins || it.role == "admin" }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        verticalArrangement = Arrangement.Top,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = if (onlyAdmins) "Administratorzy" else "Zarządzanie użytkownikami",
            style = MaterialTheme.typography.headlineMedium
        )
        Spacer(Modifier.height(AppSizes.medium))

        when {
            isLoading -> CircularProgressIndicator()
            error != null -> {
                Text(text = error ?: "", color = MaterialTheme.colorScheme.error)
            }
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
}
