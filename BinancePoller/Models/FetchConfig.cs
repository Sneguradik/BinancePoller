namespace BinancePoller.Models;

public class FetchItem
{
    public string Symbol { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public long? LastUpdate { get; set; }
}