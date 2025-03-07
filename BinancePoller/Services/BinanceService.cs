using System.Globalization;
using System.Text.Json;
using BinancePoller.Models;

namespace BinancePoller.Services;

public interface IBinanceService
{
    Task<IEnumerable<BinanceCandle>?> GetCandles(string symbol, string period, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
}

public class BinanceService(HttpClient httpClient, ILogger<BinanceService> logger) : IBinanceService
{
    
    private IEnumerable<BinanceCandle> ParseCandle(string response)
    {
        var candles = JsonSerializer.Deserialize<List<List<JsonElement>>>(response);

        var binanceCandles = new List<BinanceCandle>();

        foreach (var c in candles)
        {
            var candle = new BinanceCandle
            {
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(c[0].GetInt64()).UtcDateTime,
                OpenPrice = double.Parse(c[1].GetString()!, CultureInfo.InvariantCulture),
                HighPrice = double.Parse(c[2].GetString()!, CultureInfo.InvariantCulture),
                LowPrice = double.Parse(c[3].GetString()!, CultureInfo.InvariantCulture),
                ClosePrice = double.Parse(c[4].GetString()!, CultureInfo.InvariantCulture),
                Volume = double.Parse(c[5].GetString()!, CultureInfo.InvariantCulture),
                CloseTime = DateTimeOffset.FromUnixTimeMilliseconds(c[6].GetInt64()).UtcDateTime,
                AssetVolume = double.Parse(c[7].GetString()!, CultureInfo.InvariantCulture),
                NumberOfTrades = c[8].GetInt32(),
                TakerBuyBaseAssetVolume = double.Parse(c[9].GetString()!, CultureInfo.InvariantCulture),
                TakerBuyQuoteAssetVolume = double.Parse(c[10].GetString()!, CultureInfo.InvariantCulture)
            };
            binanceCandles.Add(candle);
        }
        
        return binanceCandles;
    }
    public async Task<IEnumerable<BinanceCandle>?> GetCandles(string symbol, string period, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        var allCandles = new List<BinanceCandle>();

        long startMs = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
        long endMs = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();

        while (startMs < endMs)
        {
            var url = $"/api/v3/klines?symbol={symbol}&interval={period}&startTime={startMs}&limit={1000}&endTime={endMs}";

            var response = await httpClient.GetAsync(url, cancellationToken);
            
            if(!response.IsSuccessStatusCode)
            {
                logger.LogError(await response.Content.ReadAsStringAsync(cancellationToken));
                break;
            }
            var candles = ParseCandle(await response.Content.ReadAsStringAsync(cancellationToken)).ToList();

            if (candles.Count == 0) break;

            allCandles.AddRange(candles);
            
            startMs = new DateTimeOffset(candles.Last().OpenTime).ToUnixTimeMilliseconds()+1;
            
            await Task.Delay(500, cancellationToken);
        }
        
        logger.LogInformation($"Got {symbol} {allCandles.Count} candles from {startTime} to {endTime}");
        Console.WriteLine($"Got: {allCandles.Count} {symbol} {endTime}");

        return allCandles;
    }
}