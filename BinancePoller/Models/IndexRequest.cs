using System.Text.Json.Serialization;

namespace BinancePoller.Models;

public class IndexRequest
{
    [JsonPropertyName("timestamp")]
    public string TimeStamp { get; set; }
    [JsonPropertyName("value")]
    public double Value { get; set; }
}