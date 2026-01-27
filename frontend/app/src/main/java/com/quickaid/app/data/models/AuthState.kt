import com.quickaid.app.data.models.AuthResponseDto

// Klasa reprezentująca stan procesu uwierzytelniania
sealed class AuthState {

    // Brak akcji, stan początkowy
    object Idle : AuthState()

    // Trwa ładowanie/przetwarzanie logowania lub rejestracji
    object Loading : AuthState()

    // Logowanie lub rejestracja zakończona sukcesem
    data class Success(val data: AuthResponseDto) : AuthState()

    // Wystąpił błąd podczas logowania lub rejestracji
    data class Error(val message: String) : AuthState()
}
