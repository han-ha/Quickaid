package com.quickaid.app.ui.screens

import android.content.Intent
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.core.net.toUri
import com.quickaid.app.R
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSpacing

@Composable
fun EmergencyScreen() {
    val context = LocalContext.current
    val instructions = stringResource(id = R.string.emergency_instructions)

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(
                horizontal = AppSpacing.large,
                vertical = AppSpacing.medium
            )
    ) {
        Text(
            text = "Tryb awaryjny",
            style = MaterialTheme.typography.headlineMedium,
            textAlign = TextAlign.Center,
            modifier = Modifier.fillMaxWidth(),
            color = MaterialTheme.colorScheme.onBackground
        )

        Spacer(Modifier.height(AppSpacing.large))

        Column(
            modifier = Modifier
                .weight(1f)
                .verticalScroll(rememberScrollState())
        ) {
            Text(
                text = instructions,
                style = MaterialTheme.typography.bodyMedium.copy(
                    textAlign = TextAlign.Justify
                ),
                color = MaterialTheme.colorScheme.onBackground
            )
        }

        Spacer(Modifier.height(AppSpacing.large))

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
}
