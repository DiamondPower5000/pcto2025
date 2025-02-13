package com.example.weatherapp

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch
import retrofit2.Response

class WeatherViewModel : ViewModel() {
    private val repository = WeatherRepository()

    private val _weatherData = MutableLiveData<WeatherData?>()
    val weatherData: LiveData<WeatherData?> = _weatherData

    private val _error = MutableLiveData<String?>()
    val error: LiveData<String?> = _error

    private val _isLoading = MutableLiveData<Boolean>()
    val isLoading: LiveData<Boolean> = _isLoading

    fun fetchWeatherData(city: String) {
        _isLoading.value = true
        viewModelScope.launch {
            try {
                val response: Response<WeatherData> = repository.getWeatherData(city)
                if (response.isSuccessful) {
                    _weatherData.value = response.body()
                    _error.value = null
                } else {
                    _error.value = "Errore: ${response.code()}"
                }
            } catch (e: Exception) {
                _error.value = "Errore di rete: ${e.message}"
            } finally {
                _isLoading.value = false
            }
        }
    }
}