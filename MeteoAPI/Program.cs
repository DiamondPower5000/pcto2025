using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register WeatherFetchService with an interface
builder.Services.AddHttpClient<IWeatherFetchService, WeatherFetchService>();
builder.Services.AddScoped<IWeatherStorageService, WeatherStorageService>();

// Register SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=weather.db"));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Enable serving static files
app.UseStaticFiles();

// Map endpoints
app.MapWeatherFetchEndpoints();
app.MapWeatherLatestEndpoints();

// Apply CORS policy
app.UseCors("AllowAll");

app.Run();
