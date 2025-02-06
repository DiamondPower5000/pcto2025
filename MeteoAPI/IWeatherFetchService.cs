public interface IWeatherFetchService
{
    Task<WeatherResponse> GetWeatherAsync(string city);
}
