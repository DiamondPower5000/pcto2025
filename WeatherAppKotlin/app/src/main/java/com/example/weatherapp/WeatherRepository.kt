package com.example.weatherapp

import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import retrofit2.Response

class WeatherRepository {
    private val apiService = RetrofitClient.weatherApiService

    suspend fun getWeatherData(city: String): Response<WeatherData> {
        return withContext(Dispatchers.IO) {
            apiService.getWeatherData(city)
        }
    }
}