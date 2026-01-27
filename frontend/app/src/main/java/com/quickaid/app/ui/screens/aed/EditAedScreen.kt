package com.quickaid.app.ui.screens.aed

import android.util.Log
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.KeyboardType
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import androidx.navigation.compose.currentBackStackEntryAsState
import com.quickaid.app.enums.AedType
import com.quickaid.app.ui.components.LargeButton
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

    var latitude by remember { mutableStateOf("") }
    var longitude by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var verified by remember { mutableStateOf(false) }

    LaunchedEffect(aed) {
        aed?.let {
            latitude = it.latitude.toString()
            longitude = it.longitude.toString()
            description = it.description.orEmpty()
            verified = it.verified
        }
    }

    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    LaunchedEffect(updateSuccess) {
        if (updateSuccess) {
            savedStateHandle?.set("aedsUpdated", true)
            navController.popBackStack()
            viewModel.resetUpdateSuccess()
            viewModel.clearSelected()
        }
    }

    if (isLoading && aed == null) {
        Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
            CircularProgressIndicator()
        }
        return
    }

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
                    && aed?.type == AedType.Internal
                    && description.isNotBlank()
                    && AedFormValidator.validateLatitude(latitude)
                    && AedFormValidator.validateLongitude(longitude),
            content = if (isLoading) "Zapisywanie..." else "Zapisz",
            onClick = {
                aed?.let { currentAed ->
                    val lat = latitude.toDoubleOrNull() ?: currentAed.latitude
                    val lon = longitude.toDoubleOrNull() ?: currentAed.longitude
                    val updated = currentAed.copy(
                        latitude = lat,
                        longitude = lon,
                        description = description.ifBlank { null },
                        verified = verified,
                        type = currentAed.type
                    )
                    viewModel.updateAed(updated)
                }
            }
        )
    }
}
