package com.quickaid.app.ui.theme

import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable

// Kolory trybu jasnego
private val LightColors = lightColorScheme(
    primary = BluePrimaryLight,
    secondary = BluePrimaryDark,
    error = RedPrimary,
    surface = White,
    background = White,
    onSurface = Black,
    onPrimary = White,
    onSecondary = White,
    onBackground = Black
)

// Kolory trybu ciemnego
private val DarkColors = darkColorScheme(
    primary = BluePrimaryLight,
    secondary = BluePrimaryDark,
    error = RedPrimary,
    surface = GreyPrimaryDark,
    background = GreyPrimaryDark,
    onSurface = White,
    onPrimary = White,
    onSecondary = White,
    onBackground = White
)

// Styl aplikacji
@Composable
fun AppTheme(
    darkTheme: Boolean = false,
    content: @Composable () -> Unit
) {
    val colors = if (darkTheme) DarkColors else LightColors

    MaterialTheme(
        colorScheme = colors,
        typography = AppTypography,
        content = content,
        shapes = AppShapes
    )
}



