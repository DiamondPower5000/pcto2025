// Endpoints/WeatherFetchEndpoints.cs
using Microsoft.EntityFrameworkCore;

public static class WeatherFetchEndpoints
{
    public static void MapWeatherFetchEndpoints(this WebApplication app)
    {
        // Endpoint to fetch and store weather data
        app.MapGet("/api/weather", async (string city, WeatherService weatherService, AppDbContext dbContext) =>
        {
            if (string.IsNullOrEmpty(city))
            {
                return Results.BadRequest("City parameter is required.");
            }

            var weather = await weatherService.GetWeatherAsync(city);
            if (weather == null || weather.Hourly?.Temperature2m == null || weather.Hourly.Temperature2m.Count == 0)
            {
                return Results.NotFound("Weather data not found for the provided city.");
            }

            var temperature = weather.Hourly.Temperature2m.FirstOrDefault();
            var record = new WeatherRecord
            {
                City = city,
                Description = "Weather data",
                Temperature = temperature,
                Humidity = weather.Current.RelativeHumidity,
                Precipitation = weather.Current.Precipitation,
                WindSpeed = weather.Current.WindSpeed,
                FetchedAt = DateTime.UtcNow
            };

            dbContext.WeatherRecords.Add(record);
            await dbContext.SaveChangesAsync();

            return Results.Json(record);
        });
    }
}
