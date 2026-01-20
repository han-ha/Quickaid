package com.quickaid.app.viewmodel

import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.quickaid.app.data.models.ArticleDto
import com.quickaid.app.data.repository.ArticleRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class ArticleDetailsViewModel @Inject constructor(
    private val repository: ArticleRepository,
    savedStateHandle: SavedStateHandle
) : ViewModel() {

    private val _article = MutableStateFlow<ArticleDto?>(null)
    val article: StateFlow<ArticleDto?> = _article

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    private val articleId: Int? = savedStateHandle.get<Int>("articleId")

    init {
        articleId?.let { fetchArticle(it) }
    }

    fun fetchArticle(id: Int) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                _article.value = repository.getArticleById(id)
            } catch (e: Exception) {
                _error.value = e.message ?: "Nieznany błąd"
            } finally {
                _isLoading.value = false
            }
        }
    }
}
