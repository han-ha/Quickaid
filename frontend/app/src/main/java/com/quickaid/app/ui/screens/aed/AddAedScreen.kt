package com.quickaid.app.ui.screens.aed

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.KeyboardType
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.data.models.AedDto
import com.quickaid.app.enums.AedType
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.util.AedFormValidator
import com.quickaid.app.viewmodel.AedViewModel

@Composable
fun AddAedScreen(
    navController: NavController,
    viewModel: AedViewModel = hiltViewModel()
) {
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val addSuccess by viewModel.addSuccess.collectAsState()

    var latitude by remember { mutableStateOf("") }
    var longitude by remember { mutableStateOf("") }
    var description by remember { mutableStateOf("") }
    var verified by remember { mutableStateOf(false) }

    val savedStateHandle = navController.previousBackStackEntry?.savedStateHandle

    LaunchedEffect(addSuccess) {
        if (addSuccess) {
            savedStateHandle?.set("aedsUpdated", true)
            navController.popBackStack()
            viewModel.resetAddSuccess()
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text("Dodaj AED", style = MaterialTheme.typography.headlineMedium)
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
                    && description.isNotBlank()
                    && AedFormValidator.validateLatitude(latitude)
                    && AedFormValidator.validateLongitude(longitude),
            content = if (isLoading) "Zapisywanie..." else "Dodaj",
            onClick = {
                val lat = latitude.toDoubleOrNull()
                val lon = longitude.toDoubleOrNull()
                if (lat != null && lon != null) {
                    viewModel.addAed(
                        AedDto(
                            id = null,
                            externalId = null,
                            latitude = lat,
                            longitude = lon,
                            description = description.ifBlank { null },
                            verified = verified,
                            type = AedType.Internal
                        )
                    )
                }
            }
        )
    }
}
