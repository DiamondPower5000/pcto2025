package com.example.weatherapp

import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Query

interface WeatherApiService {
    @GET("api/weather")
    suspend fun getWeatherData(@Query("city") city: String): Response<WeatherData>
}