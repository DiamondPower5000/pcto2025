using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class IssService
{
    private readonly HttpClient _httpClient;

    public IssService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IssResponse> GetIssPositionAsync()
    {
        var url = "http://api.open-notify.org/iss-now.json";
        var response = await _httpClient.GetStringAsync(url);
        return JsonConvert.DeserializeObject<IssResponse>(response);
    }
}

public class IssResponse
{
    public string Message { get; set; }
    public long Timestamp { get; set; }
    [JsonProperty("iss_position")]
    public IssPosition Position { get; set; }
}

public class IssPosition
{
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}
