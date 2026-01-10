package com.quickaid.app.ui.screens

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
    val role by sessionViewModel.role.collectAsState()

    LaunchedEffect(role) {
        when (role) {
            UserRole.ANON -> navController.navigate("welcome") { popUpTo("start") { inclusive = true } }
            else -> navController.navigate("home") { popUpTo("start") { inclusive = true } }
        }
    }
}
