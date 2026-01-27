package com.quickaid.app.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.SessionViewModel

@Composable
fun HomeScreen(
    navController: NavController,
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    // Stan z ViewModelu
    val role by sessionViewModel.role.collectAsState()
    val username by sessionViewModel.username.collectAsState()

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        verticalArrangement = Arrangement.Top,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {

        // Nagłówek zależny od roli
        Text(
            text = when (role) {
                UserRole.ANON -> "Witaj!"
                UserRole.USER -> "Witaj, $username!"
                UserRole.ADMIN -> "Witaj, $username!"
            },
            style = MaterialTheme.typography.headlineMedium
        )

        Spacer(Modifier.height(AppSizes.large))

        // Przyciski ogólne, dostępne dla wszystkich użytkowników
        SmallButton(
            onClick = { navController.navigate("emergency") },
            modifier = Modifier.fillMaxWidth(),
            content = "Tryb awaryjny"
        )

        Spacer(Modifier.height(AppSizes.small))

        SmallButton(
            onClick = { navController.navigate("aeds") },
            modifier = Modifier.fillMaxWidth(),
            content = "Mapa AED"
        )

        Spacer(Modifier.height(AppSizes.small))

        SmallButton(
            onClick = { navController.navigate("articles") },
            modifier = Modifier.fillMaxWidth(),
            content = "Materiały edukacyjne"
        )

        Spacer(Modifier.height(AppSizes.small))

        // Blok zależny od roli użytkownika
        when (role) {
            UserRole.ANON -> {
                // Przycisk do ekranu kontaktowego
                SmallButton(
                    onClick = { navController.navigate("contact") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Kontakt"
                )

                Spacer(Modifier.height(AppSizes.small))

                // Przycisk do logowania
                SmallButton(
                    onClick = { navController.navigate("login") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Zaloguj się"
                )

                Spacer(Modifier.height(AppSizes.small))

                // Przycisk do rejestracji
                SmallButton(
                    onClick = { navController.navigate("register") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Zarejestruj się"
                )
            }

            UserRole.USER, UserRole.ADMIN -> {
                // Dostęp do quizów edukacyjnych
                SmallButton(
                    onClick = { navController.navigate("quizzes") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Quizy edukacyjne"
                )

                Spacer(Modifier.height(AppSizes.small))

                // Dostęp do ustawień konta
                SmallButton(
                    onClick = { navController.navigate("settings") },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Ustawienia"
                )

                // Przycisk do ekranu kontaktowego
                if (role == UserRole.USER) {
                    Spacer(Modifier.height(AppSizes.small))

                    SmallButton(
                        onClick = { navController.navigate("contact") },
                        modifier = Modifier.fillMaxWidth(),
                        content = "Kontakt"
                    )
                }

                Spacer(Modifier.height(AppSizes.small))

                // Przycisk wylogowania
                SmallButton(
                    onClick = {
                        sessionViewModel.logout()
                        navController.navigate("welcome") {
                            popUpTo(0) { inclusive = true }
                            launchSingleTop = true
                        }

                    },
                    modifier = Modifier.fillMaxWidth(),
                    content = "Wyloguj"
                )

            }
        }
    }
}
