using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IssService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=issposition.db"));

var app = builder.Build();

// Endpoint to get the ISS position and return it as JSON
app.MapGet("/api/iss-position", async (IssService issService, AppDbContext dbContext) =>
{
    var issPosition = await issService.GetIssPositionAsync();
    var latitude = issPosition.Position.Latitude;
    var longitude = issPosition.Position.Longitude;

    dbContext.IssPositions.Add(new IssPositionRecord
    {
        Latitude = latitude,
        Longitude = longitude,
        Timestamp = DateTime.UtcNow
    });
    await dbContext.SaveChangesAsync();

    return Results.Json(new
    {
        latitude,
        longitude
    });
});

// Endpoint to display ISS position on a map
app.MapGet("/", async (IssService issService) =>
{
    var issPosition = await issService.GetIssPositionAsync();

    var html = @"
    <html>
    <head>
        <title>ISS Position</title>
        <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
        <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
        <style>
            #map { height: 500px; width: 100%; }
            body { font-family: 'Arial', sans-serif; background-color: #f7f7f7; color: #333; margin: 0; padding: 0; display: flex; flex-direction: column; align-items: center; }
            h1 { color: #D14E56; text-align: center; margin-top: 20px; }
        </style>
    </head>
    <body>
        <h1>ISS Current Position</h1>
        <p>Latitude: <span id='latitude'>" + issPosition.Position.Latitude + @"</span><br>
        Longitude: <span id='longitude'>" + issPosition.Position.Longitude + @"</span></p>
        <div id='map'></div>
        <script>
            var map = L.map('map').setView([" + issPosition.Position.Latitude + "," + issPosition.Position.Longitude + @"], 4);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors'
            }).addTo(map);

            var issIcon = L.icon({
                iconUrl: 'https://upload.wikimedia.org/wikipedia/commons/thumb/d/d0/International_Space_Station.svg/32px-International_Space_Station.svg.png',
                iconSize: [32, 32],
                iconAnchor: [16, 16]
            });

            var marker = L.marker([" + issPosition.Position.Latitude + "," + issPosition.Position.Longitude + @"], {icon: issIcon})
                .addTo(map)
                .bindPopup('ISS is here!');

            setInterval(function() {
                fetch('/api/iss-position')
                    .then(response => response.json())
                    .then(data => {
                        var newLat = data.latitude;
                        var newLon = data.longitude;
                        marker.setLatLng([newLat, newLon]);
                        document.getElementById('latitude').innerText = newLat;
                        document.getElementById('longitude').innerText = newLon;
                    });
            }, 5000);
        </script>
    </body>
    </html>";

    return Results.Content(html, "text/html");
});

app.Run();
