using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class WeatherStorageServiceTests
{
    private DbContextOptions<AppDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Use a unique DB per test
            .Options;
    }

    [Fact]
    public async Task StoreWeatherDataAsync_SavesRecordSuccessfully()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using (var context = new AppDbContext(options))
        {
            var storageService = new WeatherStorageService(context);
            var record = new WeatherRecord
            {
                City = "Rome",
                Description = "Partly Cloudy",
                Temperature = 18.3f,
                Humidity = 65,
                Precipitation = 0.3f,
                WindSpeed = 7.1f,
                FetchedAt = DateTime.UtcNow
            };

            // Act
            await storageService.StoreWeatherDataAsync(record);
            var storedRecord = await context.WeatherRecords.FirstOrDefaultAsync(r => r.City == "Rome");

            // Assert
            Assert.NotNull(storedRecord);
            Assert.Equal("Rome", storedRecord.City);
            Assert.Equal("Partly Cloudy", storedRecord.Description);
        }
    }

    [Fact]
    public async Task GetLatestWeatherDataAsync_ReturnsNull_WhenNoRecordExists()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using (var context = new AppDbContext(options))
        {
            var storageService = new WeatherStorageService(context);

            // Act
            var result = await storageService.GetLatestWeatherDataAsync("UnknownCity");

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task GetLatestWeatherDataAsync_OverwritesOlderRecord()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using (var context = new AppDbContext(options))
        {
            var storageService = new WeatherStorageService(context);

            var oldRecord = new WeatherRecord
            {
                City = "Paris",
                Description = "Rainy",
                Temperature = 15.0f,
                Humidity = 70,
                Precipitation = 5.0f,
                WindSpeed = 10.0f,
                FetchedAt = DateTime.UtcNow.AddHours(-2)
            };

            var newRecord = new WeatherRecord
            {
                City = "Paris",
                Description = "Clear Sky",
                Temperature = 22.0f,
                Humidity = 55,
                Precipitation = 0.0f,
                WindSpeed = 5.0f,
                FetchedAt = DateTime.UtcNow
            };

            await storageService.StoreWeatherDataAsync(oldRecord);
            await storageService.StoreWeatherDataAsync(newRecord);

            // Act
            var result = await storageService.GetLatestWeatherDataAsync("Paris");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Paris", result.City);
            Assert.Equal("Clear Sky", result.Description); // Ensures newest record is retrieved
        }
    }

    [Fact]
    public async Task StoreWeatherDataAsync_AllowsPartialData()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using (var context = new AppDbContext(options))
        {
            var storageService = new WeatherStorageService(context);
            var record = new WeatherRecord
            {
                City = "Berlin",
                Description = "Foggy",
                FetchedAt = DateTime.UtcNow // Missing temperature, humidity, precipitation, wind speed
            };

            // Act
            await storageService.StoreWeatherDataAsync(record);
            var storedRecord = await context.WeatherRecords.FirstOrDefaultAsync(r => r.City == "Berlin");

            // Assert
            Assert.NotNull(storedRecord);
            Assert.Equal("Berlin", storedRecord.City);
            Assert.Equal("Foggy", storedRecord.Description);
            Assert.Null(storedRecord.Temperature); // Ensures missing values don't cause failure
        }
    }
}
