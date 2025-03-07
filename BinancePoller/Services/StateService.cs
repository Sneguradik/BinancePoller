using BinancePoller.Models;
using Microsoft.Extensions.Options;

namespace BinancePoller.Services;

public interface IStateService
{
    Dictionary<string, DateTime> LastUpdate { get; set; }
}

public class StateService : IStateService
{
    public Dictionary<string, DateTime> LastUpdate { get; set; } = new();

    public StateService(IOptions<List<FetchItem>> fetchItems)
    {
        var dt = DateTime.UtcNow;
        foreach (var item in fetchItems.Value)
        {
            LastUpdate.Add(item.Symbol, item.LastUpdate != null?DateTimeOffset.FromUnixTimeSeconds(item.LastUpdate.Value).UtcDateTime: dt);
        }
            
    }
}
