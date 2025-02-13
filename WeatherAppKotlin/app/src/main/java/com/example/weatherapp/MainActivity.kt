package com.example.weatherapp

import android.os.Build
import android.os.Bundle
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.ViewModelProvider
import com.example.weatherapp.databinding.ActivityMainBinding
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding
    private lateinit var viewModel: WeatherViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        viewModel = ViewModelProvider(this)[WeatherViewModel::class.java]

        binding.fetchButton.setOnClickListener {
            val city = binding.cityEditText.text.toString()
            if (city.isNotEmpty()) {
                binding.progressBar.visibility = View.VISIBLE
                binding.cityTextView.visibility = View.GONE
                binding.temperatureTextView.visibility = View.GONE
                binding.humidityTextView.visibility = View.GONE
                viewModel.fetchWeatherData(city)

            }
        }

        viewModel.weatherData.observe(this) { weatherData ->
            if (weatherData != null) {
                binding.cityTextView.text = "Città: ${weatherData.city}"
                binding.temperatureTextView.text = "Temperatura: ${weatherData.temperature}°C"
                binding.humidityTextView.text = "Umidità: ${weatherData.humidity}%"
                binding.precipitationTextView.text = "Precipitazioni: ${weatherData.precipitation} mm"
                binding.windSpeedTextView.text = "Velocità del vento: ${weatherData.windSpeed} km/h"


                binding.errorTextView.visibility = View.GONE
                binding.progressBar.visibility = View.GONE
                binding.cityTextView.visibility = View.VISIBLE
                binding.temperatureTextView.visibility = View.VISIBLE
                binding.humidityTextView.visibility = View.VISIBLE
                binding.progressBar.visibility = View.GONE
            }
        }

        viewModel.error.observe(this) { error ->
            if (error != null) {
                binding.errorTextView.text = error
                binding.errorTextView.visibility = View.VISIBLE
            } else {
                binding.errorTextView.visibility = View.GONE
            }
        }

        viewModel.isLoading.observe(this) { isLoading ->
            binding.progressBar.visibility = if (isLoading) View.VISIBLE else View.GONE
        }
    }
}