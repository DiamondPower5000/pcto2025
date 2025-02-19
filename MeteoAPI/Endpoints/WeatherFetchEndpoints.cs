public static class WeatherFetchEndpoints
{
    public static void MapWeatherFetchEndpoints(this WebApplication app)
    {
        app.MapGet("/api/weather", FetchWeather);
    }

    public static async Task<IResult> FetchWeather(string city, IWeatherFetchService weatherService, IWeatherStorageService storageService)
    {
        if (string.IsNullOrEmpty(city))
        {
            return Results.BadRequest("City parameter is required.");
        }

        var weather = await weatherService.GetWeatherAsync(city);
        var record = new WeatherRecord
        {
            City = city,
            Description = "Weather data",
            Temperature = Math.Round(weather.Hourly.Temperature2m.FirstOrDefault()),
            Humidity = weather.Current.RelativeHumidity,
            Precipitation = Math.Round(weather.Current.Precipitation),
            WindSpeed = Math.Round(weather.Current.WindSpeed, 1),
            FetchedAt = DateTime.UtcNow
        };

        await storageService.StoreWeatherDataAsync(record);
        return Results.Json(record);
    }
}
