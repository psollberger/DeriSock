namespace DeriSock.Model;

using Newtonsoft.Json;

public class TradingViewMarketData
{
  /// <summary>
  ///   List of prices at close (one per candle)
  /// </summary>
  [JsonProperty("close")]
  public decimal[] Close { get; set; }

  /// <summary>
  ///   List of cost bars (volume in quote currency, one per candle)
  /// </summary>
  [JsonProperty("cost")]
  public decimal[] Cost { get; set; }

  /// <summary>
  ///   List of highest price levels (one per candle)
  /// </summary>
  [JsonProperty("high")]
  public decimal[] High { get; set; }

  /// <summary>
  ///   List of lowest price levels (one per candle)
  /// </summary>
  [JsonProperty("low")]
  public decimal[] Low { get; set; }

  /// <summary>
  ///   List of prices at open (one per candle)
  /// </summary>
  [JsonProperty("open")]
  public decimal[] Open { get; set; }

  /// <summary>
  ///   Status of the query: ok or no_data
  /// </summary>
  [JsonProperty("status")]
  public string Status { get; set; }

  /// <summary>
  ///   Values of the time axis given in milliseconds since UNIX epoch
  /// </summary>
  [JsonProperty("ticks")]
  public long[] Ticks { get; set; }

  /// <summary>
  ///   List of volume bars (in base currency, one per candle)
  /// </summary>
  [JsonProperty("volume")]
  public decimal[] Volume { get; set; }
}
