using System.Text.Json;
using System.Text.Json.Serialization;
using BinancePoller.Models;

namespace BinancePoller.Services;

public interface IIndexService
{
    Task<bool> SendCandlesAsync(IEnumerable<BinanceCandle> candles, string url, CancellationToken cancellationToken = default);
}

public class IndexService(HttpClient httpClient, ILogger<IndexService> logger) : IIndexService
{
    private readonly string _apiKey = Environment.GetEnvironmentVariable("API_KEY")!;
    
    public async Task<bool> SendCandlesAsync(IEnumerable<BinanceCandle> candles, string url, CancellationToken cancellationToken = default)
    {
        var data = candles.Select(x => new IndexRequest()
        {
            TimeStamp = x.CloseTime
                .AddMinutes(60 - x.CloseTime.Minute)
                .AddSeconds(-x.CloseTime.Second)
                .AddMilliseconds(-x.CloseTime.Millisecond)
                .ToString("O"),
            Value = x.ClosePrice
        });
        foreach (var candle in data) logger.LogInformation($"Sending candles to {url}: {candle.TimeStamp}");
        var msg = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(data.ToArray())),
        };
        msg.Content.Headers.ContentType = new("application/json");
        msg.Headers.TryAddWithoutValidation("APIKEY", _apiKey);
        var response = await httpClient.SendAsync(msg, cancellationToken);
        if (!response.IsSuccessStatusCode)logger.LogError(await response.Content.ReadAsStringAsync(cancellationToken) + response.StatusCode);
        return response.IsSuccessStatusCode;
    }
}