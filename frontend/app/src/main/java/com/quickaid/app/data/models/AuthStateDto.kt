import com.quickaid.app.data.models.AuthResponseDto

sealed class AuthStateDto {
    object Idle : AuthStateDto()
    object Loading : AuthStateDto()
    data class Success(val data: AuthResponseDto) : AuthStateDto()
    data class Error(val message: String) : AuthStateDto()
}