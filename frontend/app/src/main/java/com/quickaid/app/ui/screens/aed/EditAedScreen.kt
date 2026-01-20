package com.quickaid.app.ui.screens.aed

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.Checkbox
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
import androidx.compose.ui.text.input.KeyboardType
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.AedViewModel

@Composable
fun EditAedScreen(
    navController: NavController,
    id: Int?,
    externalId: Long?,
    viewModel: AedViewModel = hiltViewModel()
) {
    val aed by viewModel.selectedAed.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val updateSuccess by viewModel.updateSuccess.collectAsState()

    var latitude by remember { mutableStateOf("") }
    var longitude by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var verified by remember { mutableStateOf(false) }

    LaunchedEffect(id) {
        id?.let { viewModel.fetchAedById(it) }
    }

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
            enabled = !isLoading && aed != null && description.isNotBlank() && latitude.isNotBlank() && longitude.isNotBlank(),
            content = if (isLoading) "Zapisywanie..." else "Zapisz",
            onClick = {
                val currentAed = aed
                val lat = latitude.toDoubleOrNull()
                val lon = longitude.toDoubleOrNull()

                if (currentAed != null) {
                    val updated = currentAed.copy(
                        latitude = lat ?: currentAed.latitude,
                        longitude = lon ?: currentAed.longitude,
                        description = description.ifBlank { null },
                        verified = verified
                    )
                    viewModel.updateAed(updated)
                }
            }
        )

    }
}
