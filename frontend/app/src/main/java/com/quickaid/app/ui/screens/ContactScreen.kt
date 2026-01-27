package com.quickaid.app.ui.screens

import android.content.Intent
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.ClickableText
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.SpanStyle
import androidx.compose.ui.text.buildAnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.withStyle
import androidx.core.net.toUri
import androidx.navigation.NavController
import com.quickaid.app.R
import com.quickaid.app.ui.components.CustomIconButton
import com.quickaid.app.ui.theme.AppSizes

@Composable
fun ContactScreen(navController: NavController) {
    val context = LocalContext.current
    val email = stringResource(id = R.string.contact_email)
    val description = stringResource(id = R.string.contact_description)

    Box(modifier = Modifier.fillMaxSize()) {
        // Kolumna z nagłówkiem i tekstem kontaktowym
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(horizontal = AppSizes.large, vertical = AppSizes.medium),
            verticalArrangement = Arrangement.Top,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = "Kontakt",
                style = MaterialTheme.typography.headlineMedium,
                color = MaterialTheme.colorScheme.onBackground,
                textAlign = TextAlign.Center,
                modifier = Modifier.fillMaxWidth()
            )

            Spacer(Modifier.height(AppSizes.large))

            // Klikalny tekst z e-mailem
            val annotatedText = buildAnnotatedString {
                append("$description\n\n")
                pushStringAnnotation(tag = "EMAIL", annotation = "mailto:$email")
                withStyle(
                    style = SpanStyle(
                        color = MaterialTheme.colorScheme.primary,
                        fontWeight = FontWeight.Bold
                    )
                ) {
                    append(email)
                }
                pop()
            }

            ClickableText(
                text = annotatedText,
                style = MaterialTheme.typography.bodyMedium.copy(
                    color = MaterialTheme.colorScheme.onBackground,
                    textAlign = TextAlign.Justify
                ),
                modifier = Modifier.fillMaxWidth(),
                onClick = { offset ->
                    annotatedText.getStringAnnotations(tag = "EMAIL", start = offset, end = offset)
                        .firstOrNull()?.let { annotation ->
                            val intent = Intent(Intent.ACTION_SENDTO).apply {
                                data = annotation.item.toUri()
                            }
                            context.startActivity(intent)
                        }
                }
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
