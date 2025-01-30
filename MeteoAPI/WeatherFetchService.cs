using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;

public class WeatherFetchService
{
    private readonly HttpClient _httpClient;

    public WeatherFetchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "WeatherApp/1.0");
    }

    private async Task<(double lat, double lon)> GetCityCoordinatesAsync(string city)
    {
        try
        {
            var encodedCity = HttpUtility.UrlEncode(city);
            var geocodeUrl = $"https://nominatim.openstreetmap.org/search?q={encodedCity}&format=json&limit=1";

            Console.WriteLine($"Geocoding URL: {geocodeUrl}"); // Debug log

            var response = await _httpClient.GetStringAsync(geocodeUrl);
            Console.WriteLine($"Geocoding Response: {response}"); // Debug log

            var locations = JsonConvert.DeserializeObject<List<NominatimResponse>>(response);

            var location = locations?.FirstOrDefault()
                ?? throw new Exception($"Location not found for city: {city}");

            Console.WriteLine($"Found coordinates: Lat={location.Lat}, Lon={location.Lon}"); // Debug log

            return (double.Parse(location.Lat, CultureInfo.InvariantCulture),
                   double.Parse(location.Lon, CultureInfo.InvariantCulture));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCityCoordinatesAsync: {ex.Message}"); // Debug log
            throw;
        }
    }

    public async Task<WeatherResponse> GetWeatherAsync(string city)
    {
        try
        {
            var (latitude, longitude) = await GetCityCoordinatesAsync(city);

            var formattedUrl = string.Format(
                CultureInfo.InvariantCulture,
                "https://api.open-meteo.com/v1/forecast?latitude={0:F6}&longitude={1:F6}&current=relative_humidity_2m,precipitation,wind_speed_10m&hourly=temperature_2m",
                latitude,
                longitude
            );

            Console.WriteLine($"Weather API URL: {formattedUrl}"); // Debug log

            var response = await _httpClient.GetStringAsync(formattedUrl);

            var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response)
                ?? throw new JsonSerializationException("Failed to deserialize weather response");

            return weatherResponse;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}"); // Debug log
            throw new Exception($"Failed to fetch weather data: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}"); // Debug log
            throw new Exception($"Failed to parse weather data: {ex.Message}", ex);
        }
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