using Microsoft.EntityFrameworkCore;

public interface IWeatherStorageService
{
    Task StoreWeatherDataAsync(WeatherRecord record);
    Task<WeatherRecord> GetLatestWeatherDataAsync(string city);
}

public class WeatherStorageService : IWeatherStorageService
{
    private readonly AppDbContext _dbContext;

    public WeatherStorageService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task StoreWeatherDataAsync(WeatherRecord record)
    {
        _dbContext.WeatherRecords.Add(record);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<WeatherRecord> GetLatestWeatherDataAsync(string city)
    {
        return await _dbContext.WeatherRecords
            .Where(w => w.City == city)
            .OrderByDescending(w => w.FetchedAt)
            .FirstOrDefaultAsync();
    }
}