package com.quickaid.app.ui.components

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import com.quickaid.app.data.models.UserDto
import com.quickaid.app.viewmodel.AdminViewModel

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
                TextButton(
                    onClick = {
                        viewModel.deleteUser(user)
                        showDeleteDialog = false
                    }
                ) {
                    Text("Usuń", color = MaterialTheme.colorScheme.error)
                }
            },
            dismissButton = {
                TextButton(onClick = { showDeleteDialog = false }) {
                    Text("Anuluj")
                }
            }
        )
    }

    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 4.dp),
        shape = MaterialTheme.shapes.small,
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surfaceVariant
        ),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp)
        ) {
            Text(user.username, style = MaterialTheme.typography.titleMedium)
            Spacer(Modifier.height(4.dp))

            Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                SmallButton(
                    onClick = { navController.navigate("editUser/${user.id}") },
                    content = "Edytuj"
                )

                val isAdmin = user.role == "admin"
                val isCurrentUser = user.id == currentUserId
                if (!isAdmin && !isCurrentUser) {
                    SmallButton(
                        onClick = { showDeleteDialog = true },
                        content = "Usuń"
                    )
                }
            }
        }
    }
}
