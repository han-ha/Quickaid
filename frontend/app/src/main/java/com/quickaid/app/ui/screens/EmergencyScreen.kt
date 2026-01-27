package com.quickaid.app.ui.screens

import android.content.Intent
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.core.net.toUri
import androidx.navigation.NavController
import com.quickaid.app.R
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes

@Composable
fun EmergencyScreen(
    navController: NavController
) {
    val context = LocalContext.current
    val instructions = stringResource(id = R.string.emergency_instructions)

    Box(modifier = Modifier.fillMaxSize()) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(horizontal = AppSizes.large, vertical = AppSizes.medium)
                .verticalScroll(rememberScrollState()),
            horizontalAlignment = Alignment.CenterHorizontally,
        ) {
            // Nagłówek
            Text(
                text = "Tryb awaryjny",
                style = MaterialTheme.typography.headlineMedium,
                textAlign = TextAlign.Center,
                modifier = Modifier.fillMaxWidth(),
                color = MaterialTheme.colorScheme.onBackground
            )

            Spacer(Modifier.height(AppSizes.large))

            // Instrukcje awaryjne
            Column(
                modifier = Modifier
                    .weight(1f)
                    .verticalScroll(rememberScrollState())
                    .fillMaxWidth(),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Text(
                    text = instructions.trimIndent(),
                    style = MaterialTheme.typography.bodyMedium.copy(
                        lineHeight = MaterialTheme.typography.bodyMedium.fontSize * 1.5
                    ),
                    textAlign = TextAlign.Start,
                    modifier = Modifier
                        .widthIn(max = AppSizes.emergencyInstructionsWidth)
                        .padding(horizontal = AppSizes.small),
                    color = MaterialTheme.colorScheme.onBackground
                )
            }

            Spacer(Modifier.height(AppSizes.large))

            // Przycisk dzwonienia na 112
            LargeButton(
                onClick = {
                    val intent = Intent(Intent.ACTION_DIAL).apply {
                        data = "tel:112".toUri()
                    }
                    context.startActivity(intent)
                },
                modifier = Modifier.fillMaxWidth(),
                content = "Zadzwoń 112",
                buttonColor = MaterialTheme.colorScheme.error
            )
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
