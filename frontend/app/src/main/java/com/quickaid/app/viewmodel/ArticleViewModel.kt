package com.quickaid.app.viewmodel

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
class ArticleViewModel @Inject constructor(
    private val repository: ArticleRepository
) : ViewModel() {

    // Lista wszystkich artykułów
    private val _articles = MutableStateFlow<List<ArticleDto>>(emptyList())
    val articles: StateFlow<List<ArticleDto>> = _articles

    // Wybrany artykuł
    private val _selectedArticle = MutableStateFlow<ArticleDto?>(null)
    val selectedArticle: StateFlow<ArticleDto?> = _selectedArticle

    // Stan ładowania
    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    // Przechowuje komunikat błędu
    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Flagi sukcesu operacji
    private val _addSuccess = MutableStateFlow(false)
    val addSuccess: StateFlow<Boolean> = _addSuccess

    private val _updateSuccess = MutableStateFlow(false)
    val updateSuccess: StateFlow<Boolean> = _updateSuccess

    private val _deleteSuccess = MutableStateFlow(false)
    val deleteSuccess: StateFlow<Boolean> = _deleteSuccess

    // Funkcje resetujące flagi sukcesu
    fun resetAddState() {
        _addSuccess.value = false
    }

    fun resetUpdateState() {
        _updateSuccess.value = false
    }

    fun resetDeleteState() {
        _deleteSuccess.value = false
    }

    // Pobranie wszystkich artykułów
    fun fetchArticles() {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                _articles.value = repository.getArticles()
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Pobranie artykułu po ID
    fun fetchArticleById(articleId: Int) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                _selectedArticle.value = repository.getArticleById(articleId)
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Dodanie nowego artykułu
    fun addArticle(article: ArticleDto) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.addArticle(article)
                _addSuccess.value = true
                fetchArticles()
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Aktualizacja artykułu po ID
    fun updateArticle(articleId: Int, article: ArticleDto) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.updateArticle(articleId, article)
                _updateSuccess.value = true
                _selectedArticle.value = null
                fetchArticles()
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }

    // Usunięcie artykułu po ID
    fun deleteArticle(articleId: Int) {
        if (_isLoading.value) return
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            try {
                repository.deleteArticle(articleId)
                _deleteSuccess.value = true
                fetchArticles()
            } catch (e: Exception) {
                _error.value = e.message
            } finally {
                _isLoading.value = false
            }
        }
    }
}
