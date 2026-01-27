package com.quickaid.app

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.quickaid.app.ui.screens.ContactScreen
import com.quickaid.app.ui.screens.EmergencyScreen
import com.quickaid.app.ui.screens.HomeScreen
import com.quickaid.app.ui.screens.LoginScreen
import com.quickaid.app.ui.screens.RegisterScreen
import com.quickaid.app.ui.screens.SettingsScreen
import com.quickaid.app.ui.screens.StartScreen
import com.quickaid.app.ui.screens.WelcomeScreen
import com.quickaid.app.ui.screens.aed.AddAedScreen
import com.quickaid.app.ui.screens.aed.AedMapScreen
import com.quickaid.app.ui.screens.aed.EditAedScreen
import com.quickaid.app.ui.screens.articles.AddArticleScreen
import com.quickaid.app.ui.screens.articles.ArticleDetailsScreen
import com.quickaid.app.ui.screens.articles.ArticleListScreen
import com.quickaid.app.ui.screens.articles.EditArticleScreen
import com.quickaid.app.ui.screens.questions.AddQuestionScreen
import com.quickaid.app.ui.screens.questions.EditQuestionScreen
import com.quickaid.app.ui.screens.quizzes.AddQuizScreen
import com.quickaid.app.ui.screens.quizzes.EditQuizScreen
import com.quickaid.app.ui.screens.quizzes.QuizDetailsScreen
import com.quickaid.app.ui.screens.quizzes.QuizListScreen
import com.quickaid.app.ui.screens.quizzes.QuizOverviewScreen
import com.quickaid.app.ui.screens.users.EditUserScreen
import com.quickaid.app.ui.screens.users.UserListScreen
import com.quickaid.app.ui.theme.AppTheme
import com.quickaid.app.util.JwtUtils
import com.quickaid.app.viewmodel.SessionViewModel
import dagger.hilt.android.AndroidEntryPoint
import java.net.URLDecoder
import javax.inject.Inject

@AndroidEntryPoint
class MainActivity : ComponentActivity() {
    @Inject
    lateinit var jwtUtils: JwtUtils

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContent {
            // Pobranie ViewModelu sesji
            val sessionViewModel: SessionViewModel = hiltViewModel()
            val darkModeEnabled by sessionViewModel.darkModeEnabled.collectAsState()

            // Ustawienie motywu aplikacji
            AppTheme(darkTheme = darkModeEnabled) {
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    // Nawigacja całej aplikacji
                    AppNavigation()
                }
            }
        }
    }
}

@Composable
fun AppNavigation() {
    val navController = rememberNavController()
    val sessionViewModel: SessionViewModel = hiltViewModel()

    NavHost(
        navController = navController,
        startDestination = "start"
    ) {
        // Ekrany startowe
        composable("start") { StartScreen(navController, sessionViewModel) }
        composable("welcome") { WelcomeScreen(navController, sessionViewModel) }

        // Ekrany logowania i rejestracji
        composable("login") {
            LoginScreen(
                onSuccess = {
                    navController.navigate("home") { popUpTo("welcome") { inclusive = true } }
                }
            )
        }

        composable("register") {
            RegisterScreen(
                viewModel = hiltViewModel(),
                onSuccess = {
                    navController.navigate("login") { popUpTo("register") { inclusive = true } }
                }
            )
        }

        // Główny ekran aplikacji
        composable("home") { HomeScreen(navController, sessionViewModel) }

        // Ekrany pomocy i kontaktu
        composable("emergency") { EmergencyScreen(navController) }
        composable("contact") { ContactScreen(navController) }

        // Ekrany artykułów
        composable("articles") { ArticleListScreen(navController) }
        composable(
            route = "articleDetails/{articleId}",
            arguments = listOf(navArgument("articleId") { type = NavType.IntType })
        ) { backStackEntry ->
            ArticleDetailsScreen(backStackEntry = backStackEntry)
        }

        composable("addArticle") {
            AddArticleScreen(
                navController = navController
            )
        }

        composable(
            route = "editArticle/{articleId}",
            arguments = listOf(navArgument("articleId") { type = NavType.IntType })
        ) { backStackEntry ->
            val articleId = backStackEntry.arguments?.getInt("articleId") ?: 0
            EditArticleScreen(
                navController = navController,
                articleId = articleId
            )
        }

        // Ekrany AED
        composable("aeds") { AedMapScreen(navController) }
        composable("addAed") { AddAedScreen(navController) }
        composable("editAed") { EditAedScreen(navController) }

        // Ustawienia
        composable("settings") { SettingsScreen(navController, sessionViewModel) }

        // Ekrany quizów
        composable("quizzes") { QuizListScreen(navController) }

        composable(
            route = "quizOverview/{quizId}/{quizTitle}/{quizMaxScore}",
            arguments = listOf(
                navArgument("quizId") { type = NavType.IntType },
                navArgument("quizTitle") { type = NavType.StringType },
                navArgument("quizMaxScore") { type = NavType.IntType }
            )
        ) { backStackEntry ->
            val quizId = backStackEntry.arguments?.getInt("quizId") ?: 0
            val quizTitle = backStackEntry.arguments?.getString("quizTitle")?.let { URLDecoder.decode(it, "UTF-8") } ?: ""
            val quizMaxScore = backStackEntry.arguments?.getInt("quizMaxScore") ?: 0

            QuizOverviewScreen(
                quizId = quizId,
                quizTitle = quizTitle,
                quizMaxScore = quizMaxScore,
                navController = navController
            )
        }

        composable(
            "quizDetails/{quizId}",
            arguments = listOf(navArgument("quizId") { type = NavType.IntType })
        ) { backStackEntry ->
            val quizId = backStackEntry.arguments?.getInt("quizId") ?: 0
            QuizDetailsScreen(quizId, navController)
        }

        composable("addQuiz") {
            AddQuizScreen(
                navController = navController
            )
        }

        composable(
            route = "editQuiz/{quizId}",
            arguments = listOf(navArgument("quizId") { type = NavType.IntType })
        ) { backStackEntry ->
            val quizId = backStackEntry.arguments?.getInt("quizId") ?: 0
            EditQuizScreen(
                navController = navController,
                quizId = quizId
            )
        }

        // Ekrany pytań
        composable(
            route = "addQuestion/{quizId}",
            arguments = listOf(navArgument("quizId") { type = NavType.IntType })
        ) { backStackEntry ->
            val quizId = backStackEntry.arguments?.getInt("quizId") ?: 0
            AddQuestionScreen(
                navController = navController,
                quizId = quizId
            )
        }

        composable(
            route = "editQuestion/{questionId}/{quizId}",
            arguments = listOf(
                navArgument("questionId") { type = NavType.IntType },
                navArgument("quizId") { type = NavType.IntType }
            )
        ) { backStackEntry ->
            val questionId = backStackEntry.arguments?.getInt("questionId") ?: 0
            val quizId = backStackEntry.arguments?.getInt("quizId") ?: 0
            EditQuestionScreen(
                navController = navController,
                questionId = questionId,
                quizId = quizId,
            )
        }

        // Ekrany zarządzania użytkownikami
        composable("userManagement") { UserListScreen(navController) }
        composable("adminList") { UserListScreen(navController, onlyAdmins = true) }

        composable(
            "editUser/{userId}",
            arguments = listOf(navArgument("userId") { type = NavType.IntType })
        ) { backStackEntry ->
            val userId = backStackEntry.arguments?.getInt("userId") ?: 0
            EditUserScreen(userId = userId, navController = navController)
        }
    }
}
