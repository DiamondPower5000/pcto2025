// Endpoints/WeatherLatestEndpoints.cs
using Microsoft.EntityFrameworkCore;

public static class WeatherLatestEndpoints
{
    public static void MapWeatherLatestEndpoints(this WebApplication app)
    {
        // Endpoint to display the last fetched weather data for a city
        app.MapGet("/api/weather/latest", async (string city, AppDbContext dbContext) =>
        {
            var latestRecord = await dbContext.WeatherRecords
                .Where(w => w.City == city)
                .OrderByDescending(w => w.FetchedAt)
                .FirstOrDefaultAsync();

            if (latestRecord == null) return Results.NotFound("No data available for this city");

            return Results.Json(latestRecord);
        });
    }
}
