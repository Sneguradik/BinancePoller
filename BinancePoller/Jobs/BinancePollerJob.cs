using BinancePoller.Models;
using BinancePoller.Services;
using Microsoft.Extensions.Options;
using Quartz;

namespace BinancePoller.Jobs;

public class BinancePollerJob(IBinanceService binanceService, IIndexService indexService, IStateService stateService, IOptions<List<FetchItem>> fetchItems, ILogger<BinancePollerJob> logger) : IJob
{
    private async Task ProcessItem(FetchItem fetchItem, CancellationToken cancellationToken = default)
    {
        var candles = await binanceService.GetCandles(fetchItem.Symbol,
            fetchItem.Period,
            stateService.LastUpdate[fetchItem.Symbol].AddMilliseconds(1),
            DateTime.UtcNow, cancellationToken);
        if (candles is null) return;
        var binanceCandles = candles.ToList();
        if (binanceCandles.Count == 0) return;
        
        var lastCandle = binanceCandles.Last();
        if (lastCandle.CloseTime > DateTime.UtcNow)
        {
            stateService.LastUpdate[fetchItem.Symbol] = lastCandle.OpenTime;
            binanceCandles.Remove(lastCandle);
        }
        else stateService.LastUpdate[fetchItem.Symbol] = 
            binanceCandles
                .LastOrDefault()?.OpenTime
                .AddMilliseconds(1)??DateTime.UtcNow;
        if (binanceCandles.Count == 0) return;
        var result = await indexService.SendCandlesAsync(binanceCandles, fetchItem.Url, cancellationToken);
        if (result) logger.LogInformation($"{fetchItem.Symbol}: Successfully sent");
        else logger.LogError($"{fetchItem.Symbol}: Failed to send");
    }
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation($"BinancePollerJob started {DateTime.UtcNow}");
        var tasks = fetchItems.Value.Select(x=> ProcessItem(x, context.CancellationToken));
        await Task.WhenAll(tasks);
    }
}