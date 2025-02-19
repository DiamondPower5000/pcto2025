package com.example.weatherapp

import com.google.gson.annotations.SerializedName

data class WeatherData(
    @SerializedName("id") val id: Int,
    @SerializedName("city") val city: String,
    @SerializedName("description") val description: String,
    @SerializedName("temperature") val temperature: Double,
    @SerializedName("humidity") val humidity: Int,
    @SerializedName("precipitation") val precipitation: Double,
    @SerializedName("windSpeed") val windSpeed: Double,
    @SerializedName("fetchedAt") val fetchedAt: String
)