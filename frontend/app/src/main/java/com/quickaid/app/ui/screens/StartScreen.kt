package com.quickaid.app.ui.screens

import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.enums.UserRole
import com.quickaid.app.viewmodel.SessionViewModel

@Composable
fun StartScreen(
    navController: NavController,
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    // Stan z ViewModelu
    val role by sessionViewModel.role.collectAsState()

    // StartScreen służy wyłącznie do decydowania, od jakiego ekranu użytkownik powinien rozpocząć
    LaunchedEffect(role) {
        when (role) {
            UserRole.ADMIN -> navController.navigate("home") { popUpTo("start") { inclusive = true } }
            UserRole.USER -> navController.navigate("home") { popUpTo("start") { inclusive = true } }
            else -> navController.navigate("welcome") { popUpTo("start") { inclusive = true } }
        }
    }
}
