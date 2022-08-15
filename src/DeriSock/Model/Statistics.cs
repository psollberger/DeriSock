namespace DeriSock.Model;

using Newtonsoft.Json;

public class Statistics
{
  /// <summary>
  ///   highest price during 24h
  /// </summary>
  [JsonProperty("high")]
  public decimal? High { get; set; }

  /// <summary>
  ///   lowest price during 24h
  /// </summary>
  [JsonProperty("low")]
  public decimal? Low { get; set; }

  /// <summary>
  ///   24-hour price change expressed as a percentage, null if there weren't any trades
  /// </summary>
  [JsonProperty("price_change")]
  public decimal? PriceChange { get; set; }

  /// <summary>
  ///   volume during last 24h in base currency
  /// </summary>
  [JsonProperty("volume")]
  public decimal? Volume { get; set; }
}
