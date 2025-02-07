using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class WeatherStorageServiceTests
{
    [Fact]
    public async Task StoreWeatherDataAsync_SavesRecordSuccessfully()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new AppDbContext(options))
        {
            var storageService = new WeatherStorageService(dbContext);
            var record = new WeatherRecord
            {
                City = "Milan",
                Description = "Sunny",
                Temperature = 25,
                Humidity = 40,
                Precipitation = 0,
                WindSpeed = 5,
                FetchedAt = DateTime.UtcNow
            };

            // Act
            await storageService.StoreWeatherDataAsync(record);
            var savedRecord = await dbContext.WeatherRecords.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(savedRecord);
            Assert.Equal("Milan", savedRecord.City);
            Assert.Equal(25, savedRecord.Temperature);
        }
    }

    [Fact]
    public async Task GetLatestWeatherDataAsync_ReturnsMostRecentRecord()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "WeatherTestDB")
            .Options;

        using (var context = new AppDbContext(options))
        {
            var storageService = new WeatherStorageService(context);

            var oldRecord = new WeatherRecord
            {
                City = "Milan",
                Description = "Cloudy", // Ensure required field is populated
                Temperature = 20.5f,
                Humidity = 60,
                Precipitation = 1.2f,
                WindSpeed = 10.5f,
                FetchedAt = DateTime.UtcNow.AddHours(-1) // Older record
            };

            var newRecord = new WeatherRecord
            {
                City = "Milan",
                Description = "Sunny", // Ensure required field is populated
                Temperature = 25.5f,
                Humidity = 50,
                Precipitation = 0.5f,
                WindSpeed = 5.5f,
                FetchedAt = DateTime.UtcNow // Newer record
            };

            await storageService.StoreWeatherDataAsync(oldRecord);
            await storageService.StoreWeatherDataAsync(newRecord);

            // Act
            var result = await storageService.GetLatestWeatherDataAsync("Milan");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Milan", result.City);
            Assert.Equal("Sunny", result.Description); // Check if latest record is returned
        }
    }


    [Fact]
    public async Task GetLatestWeatherDataAsync_ReturnsNull_WhenNoRecordsExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using (var dbContext = new AppDbContext(options))
        {
            var storageService = new WeatherStorageService(dbContext);

            // Act
            var result = await storageService.GetLatestWeatherDataAsync("NonExistentCity");

            // Assert
            Assert.Null(result);
        }
    }
}
