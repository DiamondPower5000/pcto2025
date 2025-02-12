using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
//using Xunit;

public class WeatherFetchServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly WeatherFetchService _weatherService;

    public WeatherFetchServiceTests()
    {
        // Mock HttpMessageHandler (for simulating API responses)
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://fakeapi.com") // Fake base URL
        };

        _weatherService = new WeatherFetchService(httpClient);
    }

    [Fact]
    public async Task GetWeatherAsync_ReturnsWeatherData_WhenCityIsValid()
    {
        // Arrange
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://fakeapi.com")
        };

        var weatherServiceMock = new Mock<WeatherFetchService>(httpClient);

        weatherServiceMock
            .Setup(ws => ws.GetCityCoordinatesAsync(It.IsAny<string>()))
            .ReturnsAsync((45.4642, 9.1900)); // coordinate a caso di Milano

        var fakeResponse = new WeatherResponse
        {
            Hourly = new HourlyData { Temperature2m = new List<float> { 20.5f } },
            Current = new CurrentData { RelativeHumidity = 50, Precipitation = 1, WindSpeed = 10 }
        };

        var responseJson = JsonConvert.SerializeObject(fakeResponse);
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, System.Text.Encoding.UTF8, "application/json")
        };

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await weatherServiceMock.Object.GetWeatherAsync("Milan");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20.5f, result.Hourly.Temperature2m.First());
        Assert.Equal(50, result.Current.RelativeHumidity);
        Assert.Equal(1, result.Current.Precipitation);
        Assert.Equal(10, result.Current.WindSpeed);
    }

}
