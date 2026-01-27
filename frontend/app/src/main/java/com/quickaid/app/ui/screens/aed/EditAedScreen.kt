package com.quickaid.app.ui.screens.aed

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.KeyboardType
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import androidx.navigation.compose.currentBackStackEntryAsState
import com.quickaid.app.enums.AedType
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.util.AedFormValidator
import com.quickaid.app.viewmodel.AedViewModel

@Composable
fun EditAedScreen(
    navController: NavController
) {
    val currentBackStackEntry by navController.currentBackStackEntryAsState()
    val parentEntry = remember(currentBackStackEntry) {
        navController.getBackStackEntry("aeds")
    }

    val viewModel: AedViewModel = hiltViewModel(parentEntry)

    val aed by viewModel.selectedAed.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val updateSuccess by viewModel.updateSuccess.collectAsState()
    val deleteSuccess by viewModel.deleteSuccess.collectAsState()

    var latitude by remember { mutableStateOf("") }
    var longitude by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var verified by remember { mutableStateOf(false) }

    var showDeleteDialog by remember { mutableStateOf(false) }

    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    LaunchedEffect(aed) {
        aed?.let {
            latitude = it.latitude.toString()
            longitude = it.longitude.toString()
            description = it.description.orEmpty()
            verified = it.verified
        }
    }

    // Po aktualizacji AED
    LaunchedEffect(updateSuccess) {
        if (updateSuccess) {
            savedStateHandle?.set("aedsUpdated", true)
            navController.popBackStack()
            viewModel.resetUpdateSuccess()
            viewModel.clearSelected()
        }
    }

    // Po usunięciu AED
    LaunchedEffect(deleteSuccess) {
        if (deleteSuccess) {
            savedStateHandle?.set("aedsUpdated", true)
            navController.popBackStack()
            viewModel.resetDeleteSuccess()
            viewModel.clearSelected()
        }
    }

    if (isLoading && aed == null) {
        Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
            CircularProgressIndicator()
        }
        return
    }

    Box(modifier = Modifier.fillMaxSize()) {
        val currentAed = aed

        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(AppSizes.medium),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text("Edytuj AED", style = MaterialTheme.typography.headlineMedium)
            Spacer(Modifier.height(AppSizes.large))

            OutlinedTextField(
                value = latitude,
                onValueChange = { latitude = it.replace(',', '.') },
                label = { Text("Szerokość geograficzna") },
                modifier = Modifier.fillMaxWidth(),
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                singleLine = true
            )
            Spacer(Modifier.height(AppSizes.small))

            OutlinedTextField(
                value = longitude,
                onValueChange = { longitude = it.replace(',', '.') },
                label = { Text("Długość geograficzna") },
                modifier = Modifier.fillMaxWidth(),
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Decimal),
                singleLine = true
            )
            Spacer(Modifier.height(AppSizes.small))

            OutlinedTextField(
                value = description,
                onValueChange = { description = it },
                label = { Text("Opis") },
                modifier = Modifier.fillMaxWidth()
            )
            Spacer(Modifier.height(AppSizes.small))

            Row(
                verticalAlignment = Alignment.CenterVertically,
                modifier = Modifier.fillMaxWidth()
            ) {
                Checkbox(checked = verified, onCheckedChange = { verified = it })
                Spacer(Modifier.width(AppSizes.small))
                Text("Zweryfikowany")
            }

            Spacer(Modifier.height(AppSizes.large))

            if (error != null) {
                Text(error!!, color = MaterialTheme.colorScheme.error)
                Spacer(Modifier.height(AppSizes.small))
            }

            LargeButton(
                modifier = Modifier.fillMaxWidth(),
                enabled = !isLoading
                        && currentAed?.type == AedType.Internal
                        && description.isNotBlank()
                        && AedFormValidator.validateLatitude(latitude)
                        && AedFormValidator.validateLongitude(longitude),
                content = if (isLoading) "Zapisywanie..." else "Zapisz",
                onClick = {
                    currentAed?.let {
                        val lat = latitude.toDoubleOrNull() ?: it.latitude
                        val lon = longitude.toDoubleOrNull() ?: it.longitude
                        val updated = it.copy(
                            latitude = lat,
                            longitude = lon,
                            description = description.ifBlank { null },
                            verified = verified,
                            type = it.type
                        )
                        viewModel.updateAed(updated)
                    }
                }
            )
        }

        // Przycisk usuwania AED
        currentAed?.let {
            CustomIconButton(
                onClick = { showDeleteDialog = true },
                modifier = Modifier
                    .align(Alignment.TopEnd)
                    .padding(top = AppSizes.medium, end = AppSizes.medium)
                    .size(AppSizes.extraLarge),
                icon = Icons.Filled.Delete,
                contentDescription = "Usuń AED"
            )
        }

        // Dialog potwierdzający usunięcie
        if (showDeleteDialog && currentAed != null) {
            AlertDialog(
                onDismissRequest = { showDeleteDialog = false },
                title = { Text("Potwierdzenie usunięcia") },
                text = {
                    Text(
                        "Czy na pewno chcesz usunąć punkt AED \"${currentAed.description ?: "AED"}\"? " +
                                "Ta operacja jest nieodwracalna."
                    )
                },
                confirmButton = {
                    SmallButton(
                        onClick = {
                            currentAed.id?.let { viewModel.deleteAed(it) }
                            showDeleteDialog = false
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
    }
}
