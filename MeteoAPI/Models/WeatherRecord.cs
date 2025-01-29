using System.ComponentModel.DataAnnotations;

public class WeatherRecord
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    [Range(-100, 100)] // Reasonable temperature range in Celsius
    public double Temperature { get; set; }

    public double Humidity { get; set; }
    
    public double Precipitation { get; set; }   

    public double WindSpeed { get; set; }

    public DateTime FetchedAt { get; set; }
}