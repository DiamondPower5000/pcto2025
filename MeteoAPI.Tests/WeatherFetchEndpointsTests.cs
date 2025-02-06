using Microsoft.AspNetCore.Http.HttpResults;
using Moq;


public class WeatherFetchEndpointsTests
{
    [Fact]
    public async Task FetchWeather_ReturnsBadRequest_WhenCityIsEmpty()
    {
        // Arrange
        var weatherServiceMock = new Mock<IWeatherFetchService>();
        var storageServiceMock = new Mock<IWeatherStorageService>();

        // Act
        var result = await WeatherFetchEndpoints.FetchWeather("", weatherServiceMock.Object, storageServiceMock.Object);

        // Assert
        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task FetchWeather_ReturnsWeatherData_WhenCityIsValid()
    {
        // Arrange
        var weatherServiceMock = new Mock<IWeatherFetchService>();
        var storageServiceMock = new Mock<IWeatherStorageService>();

        weatherServiceMock
            .Setup(ws => ws.GetWeatherAsync(It.IsAny<string>()))
            .ReturnsAsync(new WeatherResponse
            {
                Hourly = new HourlyData { Temperature2m = new List<float> { 22.5f } },
                Current = new CurrentData
                {
                    RelativeHumidity = 55,
                    Precipitation = 2,
                    WindSpeed = 8.5f
                }
            });

        // Act
        var result = await WeatherFetchEndpoints.FetchWeather("Milan", weatherServiceMock.Object, storageServiceMock.Object);

        // Assert
        Assert.IsType<JsonHttpResult<WeatherRecord>>(result);
        var jsonResult = result as JsonHttpResult<WeatherRecord>;
        Assert.NotNull(jsonResult);
        Assert.Equal("Milan", jsonResult.Value.City);
    }
}
