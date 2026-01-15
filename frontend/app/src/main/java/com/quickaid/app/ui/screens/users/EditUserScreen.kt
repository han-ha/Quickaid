package com.quickaid.app.ui.screens.users

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.UserDto
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.components.RoleButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AdminViewModel

@Composable
fun EditUserScreen(
    userId: Int,
    navController: NavController,
    viewModel: AdminViewModel = hiltViewModel()
) {
    val users by viewModel.users.collectAsState()
    val user = users.find { it.id == userId }


    LaunchedEffect(userId) {
        if (user == null) {
            viewModel.fetchUsers()
        }
    }

    if (user == null) {
        Column(
            modifier = Modifier.fillMaxSize(),
            verticalArrangement = Arrangement.Center,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            CircularProgressIndicator()
        }
        return
    }

    var username by remember(user) { mutableStateOf(user.username) }
    var email by remember(user) { mutableStateOf(user.email) }
    var role by remember(user) { mutableStateOf(UserRole.fromString(user.role)) }

    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        verticalArrangement = Arrangement.Top,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = "Edycja użytkownika",
            style = MaterialTheme.typography.headlineMedium
        )

        Spacer(Modifier.height(AppSizes.extraLarge))

        OutlinedTextField(
            value = username,
            onValueChange = {
                username = it
            },
            label = { Text("Nazwa użytkownika") },
            modifier = Modifier.fillMaxWidth()
        )

        Spacer(Modifier.height(AppSizes.medium))

        OutlinedTextField(
            value = email,
            onValueChange = {
                email = it
            },
            label = { Text("Email") },
            modifier = Modifier.fillMaxWidth()
        )

        Spacer(Modifier.height(AppSizes.medium))

        Text("Rola użytkownika")
        Spacer(Modifier.height(AppSizes.small))

        Row(
            horizontalArrangement = Arrangement.spacedBy(AppSizes.medium),
            modifier = Modifier.fillMaxWidth()
        ) {
            RoleButton(
                text = "Użytkownik",
                isSelected = role == UserRole.USER,
                onClick = {
                    role = UserRole.USER
                }
            )
            RoleButton(
                text = "Administrator",
                isSelected = role == UserRole.ADMIN,
                onClick = {
                    role = UserRole.ADMIN
                }
            )
        }

        Spacer(Modifier.height(AppSizes.large))

        if (error != null) {
            Text(
                text = error ?: "",
                color = MaterialTheme.colorScheme.error
            )
            Spacer(Modifier.height(AppSizes.medium))
        }

        LargeButton(
            onClick = {
                val request = UserDto(
                    id = user.id,
                    username = username,
                    email = email,
                    role = role.toStorageString()
                )
                viewModel.updateUser(request) {
                    navController.previousBackStackEntry
                        ?.savedStateHandle
                        ?.set("usersUpdated", true)

                    navController.popBackStack()
                }
            },
            modifier = Modifier.fillMaxWidth(),
            enabled = !isLoading,
            content = if (isLoading) "Zapisywanie..." else "Zapisz zmiany"
        )

    }
}


