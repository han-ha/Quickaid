package com.quickaid.app.ui.screens

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import com.quickaid.app.R
import com.quickaid.app.enums.UserRole
import com.quickaid.app.ui.components.LargeButton
import com.quickaid.app.ui.theme.AppSizes
import com.quickaid.app.ui.theme.AppSpacing
import com.quickaid.app.viewmodel.SessionViewModel

@Composable
fun WelcomeScreen(
    navController: NavController,
    sessionViewModel: SessionViewModel = hiltViewModel()
) {
    val role by sessionViewModel.role.collectAsState()

    LaunchedEffect(role) {
        if (role != UserRole.ANON) {
            navController.navigate("home") {
                popUpTo("welcome") { inclusive = true }
            }
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(AppSpacing.medium),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Image(
            painter = painterResource(id = R.drawable.quickaid_logo),
            contentDescription = "Quickaid Logo",
            modifier = Modifier.size(AppSizes.logoSizeLarge)
        )

        Spacer(modifier = Modifier.height(AppSpacing.large))

        LargeButton(
            onClick = { navController.navigate("login") },
            modifier = Modifier.fillMaxWidth(),
            content = "Zaloguj się",
            buttonColor = MaterialTheme.colorScheme.primary
        )

        Spacer(modifier = Modifier.height(AppSpacing.small))

        LargeButton(
            onClick = { navController.navigate("register") },
            modifier = Modifier.fillMaxWidth(),
            content = "Zarejestruj się",
            buttonColor = MaterialTheme.colorScheme.primary
        )

        Spacer(modifier = Modifier.height(AppSpacing.small))

        LargeButton(
            onClick = {
                sessionViewModel.setRole(UserRole.ANON)
                navController.navigate("home")
            },
            modifier = Modifier.fillMaxWidth(),
            content = "Kontynuuj jako gość",
            buttonColor = MaterialTheme.colorScheme.primary
        )
    }
}
