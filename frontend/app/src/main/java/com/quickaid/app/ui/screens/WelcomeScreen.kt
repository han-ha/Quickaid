package com.quickaid.app.ui.screens

import android.content.Context
import android.content.Intent
import android.net.ConnectivityManager
import android.net.NetworkCapabilities
import android.provider.Settings
import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.*
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Text
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.R
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.components.SmallButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.viewmodel.SessionViewModel

@Composable
fun WelcomeScreen(
    navController: NavController,
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    val context = LocalContext.current

    // Stan połączenia internetowego
    var isConnected by remember { mutableStateOf(true) }
    var showEnableWifiDialog by remember { mutableStateOf(false) }

    // Funkcja sprawdzająca, czy urządzenie ma dostęp do Internetu
    fun checkInternet(): Boolean {
        val connectivityManager =
            context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
        val network = connectivityManager.activeNetwork ?: return false
        val capabilities = connectivityManager.getNetworkCapabilities(network) ?: return false
        return capabilities.hasCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET)
    }

    // Funkcja otwierająca systemowe ustawienia wifi
    fun openWifiSettings() {
        val intent = Intent(Settings.ACTION_WIFI_SETTINGS)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK
        context.startActivity(intent)
    }

    // Sprawdzenie połączenia z Internetem przy uruchomieniu ekranu
    LaunchedEffect(Unit) {
        isConnected = checkInternet()
        if (!isConnected) showEnableWifiDialog = true
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSizes.medium),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        // Logo aplikacji
        Image(
            painter = painterResource(id = R.drawable.quickaid_logo),
            contentDescription = "Quickaid Logo",
            modifier = Modifier.size(AppSizes.logoSizeLarge)
        )

        Spacer(modifier = Modifier.height(AppSizes.large))

        // Przycisk logowania
        LargeButton(
            onClick = { navController.navigate("login") },
            modifier = Modifier.fillMaxWidth(),
            content = "Zaloguj się",
            buttonColor = MaterialTheme.colorScheme.primary
        )

        Spacer(modifier = Modifier.height(AppSizes.small))

        // Przycisk rejestracji
        LargeButton(
            onClick = { navController.navigate("register") },
            modifier = Modifier.fillMaxWidth(),
            content = "Zarejestruj się",
            buttonColor = MaterialTheme.colorScheme.primary
        )

        Spacer(modifier = Modifier.height(AppSizes.small))

        // Przycisk kontynuowania jako gość
        LargeButton(
            onClick = {
                sessionViewModel.setRole(UserRole.ANON)
                navController.navigate("home") {
                    popUpTo(0) { inclusive = true }
                }
            },
            modifier = Modifier.fillMaxWidth(),
            content = "Kontynuuj jako gość",
            buttonColor = MaterialTheme.colorScheme.primary
        )
    }

    // Pop-up informujący o braku połączenia internetowego
    if (showEnableWifiDialog) {
        AlertDialog(
            onDismissRequest = {},
            title = { Text("Wymagane połączenie z Internetem") },
            text = { Text("Aplikacja wymaga połączenia z Internetem, aby działać poprawnie. Włącz Wi-Fi lub dane mobilne.") },
            confirmButton = {
                SmallButton(onClick = {
                    openWifiSettings()
                    showEnableWifiDialog = false
                }, content = "Otwórz ustawienia")
            },
            dismissButton = {
                SmallButton(
                    onClick = { showEnableWifiDialog = false },
                    content = "Anuluj")
            }
        )
    }
}
