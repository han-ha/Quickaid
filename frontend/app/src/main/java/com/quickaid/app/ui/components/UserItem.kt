package com.quickaid.app.ui.components

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import com.quickaid.app.data.models.UserDto
import com.quickaid.app.viewmodel.AdminViewModel
import com.quickaid.app.ui.theme.AppSpacing

@Composable
fun UserItem(
    user: UserDto,
    viewModel: AdminViewModel,
    navController: NavController,
    currentUserId: Int?
) {
    var showDeleteDialog by remember { mutableStateOf(false) }

    if (showDeleteDialog) {
        AlertDialog(
            onDismissRequest = { showDeleteDialog = false },
            title = { Text("Usuń użytkownika") },
            text = {
                Text("Czy na pewno chcesz usunąć użytkownika ${user.username}? Ta operacja jest nieodwracalna.")
            },
            confirmButton = {
                SmallButton(
                    onClick = {
                        viewModel.deleteUser(user)
                        navController.currentBackStackEntry
                            ?.savedStateHandle
                            ?.set("usersUpdated", true)
                        showDeleteDialog = false
                    },
                    content = "Usuń",
                    buttonColor = MaterialTheme.colorScheme.error
                )
            },
            dismissButton = {
                SmallButton(onClick = { showDeleteDialog = false }, content = "Anuluj")
            }
        )
    }

    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = AppSpacing.extraSmall),
        shape = MaterialTheme.shapes.small,
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surfaceVariant
        ),
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(AppSpacing.medium)
        ) {
            Text(user.username, style = MaterialTheme.typography.titleMedium)
            Spacer(Modifier.height(AppSpacing.extraSmall))

            val isAdmin = user.role == "admin"
            val isCurrentUser = user.id == currentUserId

            if (!isAdmin && !isCurrentUser) {
                AdminActions(
                    onEdit = { navController.navigate("editUser/${user.id}") },
                    onDelete = { showDeleteDialog = true }
                )
            } else {
                SmallButton(
                    onClick = { navController.navigate("editUser/${user.id}") },
                    content = "Edytuj"
                )
            }
        }
    }
}
