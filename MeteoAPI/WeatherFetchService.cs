using System.Globalization;
using Newtonsoft.Json;
using System.Web;

public class WeatherFetchService : IWeatherFetchService
{
    private readonly HttpClient _httpClient;

    public WeatherFetchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "WeatherApp/1.0");
    }

    private async Task<(double lat, double lon)> GetCityCoordinatesAsync(string city)
    {
        var encodedCity = HttpUtility.UrlEncode(city);
        var geocodeUrl = $"https://nominatim.openstreetmap.org/search?q={encodedCity}&format=json&limit=1";

        var response = await _httpClient.GetStringAsync(geocodeUrl);
        var locations = JsonConvert.DeserializeObject<List<NominatimResponse>>(response);

        var location = locations?.FirstOrDefault()
            ?? throw new Exception($"Location not found for city: {city}");

        return (double.Parse(location.Lat, CultureInfo.InvariantCulture),
               double.Parse(location.Lon, CultureInfo.InvariantCulture));
    }

    public async Task<WeatherResponse> GetWeatherAsync(string city)
    {
        var (latitude, longitude) = await GetCityCoordinatesAsync(city);

        var formattedUrl = string.Format(
            CultureInfo.InvariantCulture,
            "https://api.open-meteo.com/v1/forecast?latitude={0:F6}&longitude={1:F6}&current=relative_humidity_2m,precipitation,wind_speed_10m&hourly=temperature_2m",
            latitude,
            longitude
        );

        var response = await _httpClient.GetStringAsync(formattedUrl);
        var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response)
            ?? throw new JsonSerializationException("Failed to deserialize weather response");

        return weatherResponse;
    }
}

public class NominatimResponse
{
    public string Lat { get; set; }
    public string Lon { get; set; }
    public string DisplayName { get; set; }
}

public class WeatherResponse
{
    public HourlyData Hourly { get; set; }
    public CurrentData Current { get; set; }
}

public class HourlyData
{
    [JsonProperty("temperature_2m")]
    public List<float> Temperature2m { get; set; }
}

public class CurrentData
{
    [JsonProperty("relative_humidity_2m")]
    public float RelativeHumidity { get; set; }

    [JsonProperty("precipitation")]
    public float Precipitation { get; set; }

    [JsonProperty("wind_speed_10m")]
    public float WindSpeed { get; set; }
}
