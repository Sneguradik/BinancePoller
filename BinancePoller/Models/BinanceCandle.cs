namespace BinancePoller.Models;

public class BinanceCandle
{
    public DateTime OpenTime { get; set; }
    public double OpenPrice { get; set; }
    public double HighPrice { get; set; }
    public double LowPrice { get; set; }
    public double ClosePrice { get; set; }
    public double Volume { get; set; }
    public DateTime CloseTime { get; set; }
    public double AssetVolume { get; set; }
    public int NumberOfTrades { get; set; }
    public double TakerBuyBaseAssetVolume { get; set; }
    public double TakerBuyQuoteAssetVolume { get; set; }
}